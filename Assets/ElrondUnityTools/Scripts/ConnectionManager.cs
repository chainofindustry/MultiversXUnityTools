using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using System;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

namespace ElrondUnityTools
{
    public class ConnectionManager : MonoBehaviour
    {
        private static ConnectionManager instance;

        private ElrondProvider provider;
        private NetworkConfig networkConfig;


        UnityAction<AccountDto> OnWalletConnected;
        UnityAction OnWalletDisconnected;

        bool walletConnected;
        bool walletConnectInitialized;

        public static ConnectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject(Constants.ConnectionManagerObject);
                    instance = go.AddComponent<ConnectionManager>();
                  
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        internal void DeepLinkLogin()
        {
            OpenDeepLink();
        }

        internal bool IsWalletConnected()
        {
            return walletConnected;
          
        }

        internal async void Connect(UnityAction<AccountDto> OnWalletConnected, UnityAction OnWalletDisconnected, Image qrImage)
        {
            this.OnWalletConnected = OnWalletConnected;
            this.OnWalletDisconnected = OnWalletDisconnected;
            WalletConnect walletConnect = gameObject.AddComponent<WalletConnect>();
            ClientMeta appData = new ClientMeta();
            appData.Description = "You are using Chain of Industry test login";
            appData.Icons = new string[1];
            appData.Icons[0] = "https://vilas.edu.vn/wp-content/uploads/2019/07/SCSS-7-Icon2-150x150.png";
            appData.Name = "Chain of Industry";
            appData.URL = "http://chainofindustry.com/";
            walletConnect.AppData = appData;
            walletConnect.customBridgeUrl = Constants.customBridgeUrl;
            walletConnect.ConnectedEvent = new WalletConnect.WalletConnectEventNoSession();
            walletConnect.ConnectedEventSession = new WalletConnect.WalletConnectEventWithSessionData();

            walletConnectInitialized = true;
            if (qrImage != null)
            {

                qrImage.gameObject.SetActive(false);
                WalletConnectQRImage WalletConnectQRImage = qrImage.gameObject.AddComponent<WalletConnectQRImage>();
                WalletConnectQRImage.walletConnect = walletConnect;
                qrImage.gameObject.SetActive(true);
            }

            provider = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Erdcsharp.Configuration.Network.DevNet));
            networkConfig = await NetworkConfig.GetFromNetwork(provider);
        }

        public void Disconnect()
        {
            WalletConnect.Instance.CloseSession();
        }



        private void Update()
        {
            if (!walletConnectInitialized)
                return;

            if (WalletConnect.ActiveSession == null)
                return;

            if (walletConnected == true)
                return;

            if (WalletConnect.ActiveSession.Accounts != null)
            {
                OnConnected();
            }
        }


        private async void OnConnected()
        {
            walletConnected = true;
            WalletConnect.ActiveSession.OnSessionDisconnect += ActiveSessionOnDisconnect;
            AccountDto connectedAccount = await provider.GetAccount(WalletConnect.ActiveSession.Accounts[0]);
            OnWalletConnected(connectedAccount);
        }

        private void ActiveSessionOnDisconnect(object sender, EventArgs e)
        {
            Debug.Log("ActiveSessionOnDisconnect");
            WalletConnect.ActiveSession.OnSessionDisconnect -= ActiveSessionOnDisconnect;
            walletConnected = false;
            OnWalletDisconnected();
        }


        void OpenDeepLink()
        {
            if (!WalletConnect.ActiveSession.ReadyForUserPrompt)
            {
                Debug.LogError("WalletConnectUnity.ActiveSession not ready for a user prompt" +
                               "\nWait for ActiveSession.ReadyForUserPrompt to be true");

                return;
            }

#if UNITY_ANDROID

            string maiarUrl = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + UnityWebRequest.EscapeURL(WalletConnect.Instance.ConnectURL);

            Debug.Log("[WalletConnect] Opening URL: " + maiarUrl);


            Application.OpenURL(maiarUrl);
#elif UNITY_IOS
            if (SelectedWallet == null)
            {
                throw new NotImplementedException(
                    "You must use OpenDeepLink(AppEntry) or set SelectedWallet on iOS!");
            }
            else
            {
                string url;
                string encodedConnect = WebUtility.UrlEncode(ConnectURL);
                if (!string.IsNullOrWhiteSpace(SelectedWallet.mobile.universal))
                {
                    url = SelectedWallet.mobile.universal + "/wc?uri=" + encodedConnect;
                }
                else
                {
                    url = SelectedWallet.mobile.native + (SelectedWallet.mobile.native.EndsWith(":") ? "//" : "/") +
                          "wc?uri=" + encodedConnect;
                }
                
                Debug.Log("Opening: " + url);
                Application.OpenURL(url);
            }
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }
    }
}