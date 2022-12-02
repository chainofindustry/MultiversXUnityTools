using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Network;
using WalletConnectSharp.Unity.Network;
using WalletConnectSharp.Unity.Utils;

namespace MultiversXUnityTools
{
    [RequireComponent(typeof(NativeWebSocketTransport))]

    //wallet connect wrapper adapted to Maiar
    public class WalletConnect : BindableMonoBehavior
    {
        public const string SessionKey = "__WALLETCONNECT_SESSION__";

        [Serializable]
        public class WalletConnectEventNoSession : UnityEvent { }
        [Serializable]
        public class WalletConnectEventWithSession : UnityEvent<WalletConnectUnitySession> { }
        [Serializable]
        public class WalletConnectEventWithSessionData : UnityEvent<WCSessionData> { }

        public event EventHandler ConnectionStarted;
        public event EventHandler NewSessionStarted;

        [BindComponent]
        private NativeWebSocketTransport _transport;

        private static WalletConnect _instance;


        public static WalletConnectUnitySession ActiveSession
        {
            get
            {
                return _instance.Session;
            }
        }

        public string ConnectURL
        {
            get
            {
                return Session.URI;
            }
        }

        public bool retryOnTimeout = true;
        public bool autoSaveAndResume = true;
        public bool connectOnAwake = false;
        public bool connectOnStart = true;
        public bool createNewSessionOnSessionDisconnect = true;
        public int connectSessionRetryCount = 3;
        public string customBridgeUrl;

        public int chainId = 1;

        public WalletConnectEventNoSession ConnectedEvent;
        public WalletConnectEventWithSessionData ConnectedEventSession;
        public WalletConnectEventWithSession DisconnectedEvent;
        public WalletConnectEventWithSession ConnectionFailedEvent;
        public WalletConnectEventWithSession NewSessionConnected;
        public WalletConnectEventWithSession ResumedSessionConnected;

        public WalletConnectUnitySession Session
        {
            get;
            private set;
        }

        [Obsolete("Use Session instead of Protocol")]
        public WalletConnectUnitySession Protocol
        {
            get { return Session; }
            private set
            {
                Session = value;
            }
        }

        public bool Connected
        {
            get
            {
                return Session.Connected;
            }
        }

        [SerializeField]
        public ClientMeta AppData;

        protected override async void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            _instance = this;

            base.Awake();

            if (connectOnAwake)
            {
                await Connect();
            }
        }

        async void Start()
        {
            if (connectOnStart && !connectOnAwake)
            {
                await Connect();
            }
        }

        public async Task<WCSessionData> Connect()
        {
            while (true)
            {
                try
                {
                    SavedSession savedSession = null;
                    if (PlayerPrefs.HasKey(SessionKey))
                    {
                        var json = PlayerPrefs.GetString(SessionKey);
                        savedSession = JsonConvert.DeserializeObject<SavedSession>(json);
                    }

                    if (string.IsNullOrWhiteSpace(customBridgeUrl))
                    {
                        customBridgeUrl = null;
                    }

                    if (Session != null)
                    {
                        var currentKey = Session.KeyData;
                        if (savedSession != null)
                        {
                            if (currentKey != savedSession.Key)
                            {
                                if (Session.SessionConnected)
                                {
                                    await Session.Disconnect();
                                }
                                else if (Session.TransportConnected)
                                {
                                    await Session.Transport.Close();
                                }
                            }
                            else if (!Session.Connected && !Session.Connecting)
                            {

                                SetupEvents();

                                return await CompleteConnect();
                            }
                            else
                            {
                                return null; //Nothing to do
                            }
                        }
                        else if (Session.SessionConnected)
                        {
                            await Session.Disconnect();
                        }
                        else if (Session.TransportConnected)
                        {
                            await Session.Transport.Close();
                        }
                        else if (Session.Connecting)
                        {
                            //We are still connecting, do nothing
                            return null;
                        }
                    }

                    //default will be set by library
                    ICipher ciper = null;

#if UNITY_WEBGL
            ciper = new WebGlAESCipher();
#endif

                    if (savedSession != null)
                    {
                        Session = new WalletConnectUnitySession(savedSession, this, _transport);
                    }
                    else
                    {
                        Session = new WalletConnectUnitySession(AppData, this, customBridgeUrl, _transport, ciper,
                            chainId);

                        if (NewSessionStarted != null)
                            NewSessionStarted(this, EventArgs.Empty);
                    }

                    SetupEvents();

                    return await CompleteConnect();
                }
                catch (TimeoutException)
                {
                    Debug.Log("Timeout Reached, Regenerating Session");
                }
            }

        }

        private void SetupEvents()
        {
#if UNITY_EDITOR || DEBUG
            //Useful for debug logging
            Session.OnSessionConnect += (sender, session) =>
            {
                Debug.Log("[WalletConnect] Session Connected");
            };
#endif

            Session.OnSessionDisconnect += SessionOnOnSessionDisconnect;
            Session.OnSessionCreated += SessionOnOnSessionCreated;
            Session.OnSessionResumed += SessionOnOnSessionResumed;

#if UNITY_ANDROID || UNITY_IOS
            //Whenever we send a request to the Wallet, we want to open the Wallet app
            Session.OnSend += SessionOnSend; /*(sender, session) => OpenMobileWallet();*/
#endif
        }

        private void SessionOnSend(object sender, WalletConnectSession session)
        {
            Debug.Log("OnSend");
        }

        private void TeardownEvents()
        {
            Session.OnSessionDisconnect -= SessionOnOnSessionDisconnect;
            Session.OnSessionCreated -= SessionOnOnSessionCreated;
            Session.OnSessionResumed -= SessionOnOnSessionResumed;
        }

        private void SessionOnOnSessionResumed(object sender, WalletConnectSession e)
        {
            if (this.ResumedSessionConnected != null)
                this.ResumedSessionConnected.Invoke(e as WalletConnectUnitySession ?? Session);
        }

        private void SessionOnOnSessionCreated(object sender, WalletConnectSession e)
        {
            if (this.NewSessionConnected != null)
                this.NewSessionConnected.Invoke(e as WalletConnectUnitySession ?? Session);
        }

        private async Task<WCSessionData> CompleteConnect()
        {
            Debug.Log("Waiting for Wallet connection");

            if (ConnectionStarted != null)
            {
                ConnectionStarted(this, EventArgs.Empty);
            }

            WalletConnectEventWithSessionData allEvents = new WalletConnectEventWithSessionData();

            allEvents.AddListener(delegate (WCSessionData arg0)
            {
                ConnectedEvent.Invoke();
                ConnectedEventSession.Invoke(arg0);
            });

            int tries = 0;
            while (tries < connectSessionRetryCount)
            {
                try
                {
                    var session = await Session.SourceConnectSession();

                    allEvents.Invoke(session);

                    return session;
                }
                catch (IOException e)
                {
                    tries++;

                    if (tries >= connectSessionRetryCount)
                        throw new IOException("Failed to request session connection after " + tries + " times.", e);
                }
            }

            throw new IOException("Failed to request session connection after " + tries + " times.");
        }

        private async void SessionOnOnSessionDisconnect(object sender, EventArgs e)
        {
            if (DisconnectedEvent != null)
                DisconnectedEvent.Invoke(ActiveSession);

            if (autoSaveAndResume && PlayerPrefs.HasKey(SessionKey))
            {
                PlayerPrefs.DeleteKey(SessionKey);
            }

            TeardownEvents();

            if (createNewSessionOnSessionDisconnect)
            {
                await Connect();
            }
        }

        private async void OnDestroy()
        {
            await SaveOrDisconnect();
        }

        private async void OnApplicationQuit()
        {
            await SaveOrDisconnect();
        }

        private async void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                await SaveOrDisconnect();
            }
            else if (PlayerPrefs.HasKey(SessionKey) && autoSaveAndResume)
            {
                await Connect();
            }
        }

        private async Task SaveOrDisconnect()
        {
            if (!Session.Connected)
                return;

            if (autoSaveAndResume)
            {
                var session = Session.SaveSession();
                var json = JsonConvert.SerializeObject(session);
                PlayerPrefs.SetString(SessionKey, json);

                await Session.Transport.Close();
            }
            else
            {
                await Session.Disconnect();
            }
        }

        /// <summary>
        /// Maiar deep link urls
        /// </summary>
        public void OpenMobileWallet()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var signingURL = ConnectURL.Split('@')[0];
            string maiarUrl = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + UnityWebRequest.EscapeURL(signingURL);
            Debug.Log("[WalletConnect] Opening URL: " + maiarUrl);
            Application.OpenURL(maiarUrl);
#elif UNITY_IOS && !UNITY_EDITOR
            string maiarUrl = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com" + "/wc";
            Debug.Log("[WalletConnect] Opening URL: " + maiarUrl);
            Application.OpenURL(maiarUrl);
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }

        public void OpenDeepLink()
        {
            if (!ActiveSession.ReadyForUserPrompt)
            {
                Debug.LogError("WalletConnectUnity.ActiveSession not ready for a user prompt" +
                               "\nWait for ActiveSession.ReadyForUserPrompt to be true");

                return;
            }

#if UNITY_ANDROID
            string maiarUrl = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + UnityWebRequest.EscapeURL(ConnectURL);
            Debug.Log("[WalletConnect] Opening URL: " + maiarUrl);
            Application.OpenURL(maiarUrl);

#elif UNITY_IOS
            string encodedConnect = WebUtility.UrlEncode(ConnectURL);
            string maiarUrl = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + encodedConnect;
            Debug.Log("[WalletConnect] Opening URL: " + maiarUrl);
            Application.OpenURL(maiarUrl);
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }

        public void CLearSession()
        {
            PlayerPrefs.DeleteKey(SessionKey);
        }

        public async void CloseSession(bool waitForNewSession = true)
        {
            if (ActiveSession == null)
                return;

            await ActiveSession.Disconnect();

            if (waitForNewSession)
                await ActiveSession.Connect();
        }
    }
}