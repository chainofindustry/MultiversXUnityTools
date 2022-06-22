using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;

namespace ElrondUnityTools
{
    public class ConnectionManager : WalletConnectActions
    {
        private static ConnectionManager instance;

        private AccountDto connectedAccount;
        private ElrondProvider provider;
        private NetworkConfig networkConfig;
        private UnityAction<OperationStatus, string> OnTransactionStatusChanged;
        string txHash;


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

        internal async void SendTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> transactionStatus)
        {
            OnTransactionStatusChanged = transactionStatus;
            var transaction = new TransactionData()
            {
                nonce = connectedAccount.Nonce,
                from = connectedAccount.Address,
                to = "erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf",
                amount = "10000000000000000",
                data = "You see this?",
                gasPrice = networkConfig.MinGasPrice.ToString(),
                gasLimit = (networkConfig.MinGasLimit + 20000).ToString(),
                chainId = networkConfig.ChainId,
                version = networkConfig.MinTransactionVersion
            };

            OnTransactionStatusChanged(OperationStatus.InProgress, "Waiting for signing");

            var signature = await SignTransaction(transaction);

            if (signature.Contains("error"))
            {
                OnTransactionStatusChanged(OperationStatus.Error, signature);
                return;
            }

            SignedTransactionData tx = new SignedTransactionData(transaction, signature);
            string json = JsonUtility.ToJson(tx);



            StartCoroutine(PostTransaction("https://devnet-api.elrond.com/transactions", json));
        }

        IEnumerator PostTransaction(string uri, string signedData)
        {
            OnTransactionStatusChanged(OperationStatus.InProgress, "Broadcasting transaction to blockchain");

            using var webRequest = new UnityWebRequest();
            webRequest.url = uri; // PostUri is a string containing the url
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(signedData)); // postData is Json file as a string
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    OnTransactionStatusChanged(OperationStatus.Error, webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    OnTransactionStatusChanged(OperationStatus.Error, webRequest.error + " " + webRequest.result + " " + webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.Success:
                    string output = webRequest.downloadHandler.text;
                    BroadcastResponse response = JsonConvert.DeserializeObject<BroadcastResponse>(output);
                    OnTransactionStatusChanged(OperationStatus.InProgress, response.txHash);

                    txHash = response.txHash;
                    CheckStatus(response.txHash);
                    break;
            }
        }

        private void CheckStatus(string txHash)
        {
            StartCoroutine(GetTransactionStatusRequest("https://devnet-api.elrond.com/transactions/" + txHash + "?fields=status", Complete));
        }

        IEnumerator GetTransactionStatusRequest(string uri, UnityAction<UnityWebRequest.Result, string> CompleteMethod)
        {
            Debug.Log(uri);

            yield return new WaitForSeconds(1);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                string result = null;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        OnTransactionStatusChanged(OperationStatus.Error, webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        OnTransactionStatusChanged(OperationStatus.Error, webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        result = webRequest.downloadHandler.text;
                        break;
                }

                CompleteMethod(webRequest.result, result);
            }
        }


        private void Complete(UnityWebRequest.Result result, string message)
        {
            if (result == UnityWebRequest.Result.Success)
            {
                TransactionStatus status = JsonConvert.DeserializeObject<TransactionStatus>(message);

                //this.status.text = status.status;

                if (status.status != "success")
                {
                    OnTransactionStatusChanged(OperationStatus.InProgress, status.status);
                    StartCoroutine(GetTransactionStatusRequest("https://devnet-api.elrond.com/transactions/" + txHash + "?fields=status", Complete));
                }
                else
                {
                    Debug.Log("REFRESH ACCOUNT");
                    OnTransactionStatusChanged(OperationStatus.Complete, status.status);
                }
            }
        }

        public class TransactionStatus
        {
            public string status { get; set; }
        }

        public class BroadcastResponse
        {
            public string txHash;
            public string receiver;
            public string sender;
            public int receiverShard;
            public int senderShard;
            public string status;
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
            connectedAccount = await provider.GetAccount(WalletConnect.ActiveSession.Accounts[0]);
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