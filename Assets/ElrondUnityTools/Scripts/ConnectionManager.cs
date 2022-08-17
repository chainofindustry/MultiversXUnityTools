using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
        private AccountDto connectedAccount;
        private ElrondProvider provider;
        private NetworkConfig networkConfig;
        private UnityAction<OperationStatus, string> OnSigningTransactionStatusChanged;
        private UnityAction<OperationStatus, string> OnBlockchainTransactionStatusChanged;
        private UnityAction<OperationStatus, string, NFTMetadata[]> LoadNFTStatus;
        private UnityAction<AccountDto> OnWalletConnected;
        private UnityAction OnWalletDisconnected;
        private WalletConnect walletConnect;
        private bool walletConnected;
        private bool walletConnectInitialized;

        private static ConnectionManager instance;
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



        internal async void Connect(UnityAction<AccountDto> OnWalletConnected, UnityAction OnWalletDisconnected, UnityEngine.UI.Image qrImage)
        {
            this.OnWalletConnected = OnWalletConnected;
            this.OnWalletDisconnected = OnWalletDisconnected;
            walletConnect = gameObject.AddComponent<WalletConnect>();
            ClientMeta appData = new ClientMeta();
            appData.Description = Constants.appDescription;
            appData.Icons = new string[1];
            appData.Icons[0] = Constants.appIcon;
            appData.Name = Constants.appName;
            appData.URL = Constants.appWebsite;
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

            provider = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Constants.networkType));
            networkConfig = await NetworkConfig.GetFromNetwork(provider);
        }


        internal void DeepLinkLogin()
        {
            walletConnect.OpenDeepLink();
        }


        internal bool IsWalletConnected()
        {
            return walletConnected;
        }


        internal void Disconnect()
        {
            WalletConnect.Instance.CloseSession();
        }


        #region SendTransaction
        internal void SendESDTTransaction(string destinationAddress, string amount, ESDTToken token, UnityAction<OperationStatus, string> transactionStatus)
        {
            float value = float.Parse(amount);
            value = value * Mathf.Pow(10, token.decimals);
            Debug.Log(value + " " + token.identifier);
            string hexaAmount = ((long)value).ToString("X");
            if (hexaAmount.Length % 2 == 1)
            {
                hexaAmount = "0" + hexaAmount;
            }

            string hexaTokenIdentifier = Erdcsharp.Domain.Helper.Converter.ToHexString(token.identifier);

            SendTransaction(destinationAddress, 0.ToString(), "ESDTTransfer" + "@" + hexaTokenIdentifier + "@" + hexaAmount, transactionStatus, networkConfig.MinGasLimit + 500000);
        }

        internal void SendEGLDTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> TransactionStatus)
        {
            SendTransaction(destinationAddress, amount, data, TransactionStatus, networkConfig.MinGasLimit + 20000);
        }



        internal async void SendTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> transactionStatus, long gasLimit)
        {

            OnSigningTransactionStatusChanged = transactionStatus;
            var transaction = new TransactionData()
            {
                nonce = connectedAccount.Nonce,
                from = connectedAccount.Address,
                to = destinationAddress,
                amount = TokenAmount.EGLD(amount).ToString(),
                data = data,
                gasPrice = networkConfig.MinGasPrice.ToString(),
                gasLimit = gasLimit.ToString(),
                chainId = networkConfig.ChainId,
                version = networkConfig.MinTransactionVersion
            };

            OnSigningTransactionStatusChanged(OperationStatus.InProgress, "Waiting for signing");

            string signature = "";
            try
            {
                signature = await SignTransaction(transaction);
            }
            catch (IOException e)
            {
                OnSigningTransactionStatusChanged(OperationStatus.Error, e.Message + " " + e.Data);
                return;
            }

            SignedTransactionData tx = new SignedTransactionData(transaction, signature);
            string json = JsonUtility.ToJson(tx);

            StartCoroutine(PostTransaction(Constants.transactionPostAPI, json));
        }


        private IEnumerator PostTransaction(string url, string signedData)
        {
            OnSigningTransactionStatusChanged(OperationStatus.InProgress, "Broadcasting transaction to blockchain");

            using var webRequest = new UnityWebRequest();
            webRequest.url = url;
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(signedData));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    OnSigningTransactionStatusChanged(OperationStatus.Error, webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    OnSigningTransactionStatusChanged(OperationStatus.Error, webRequest.error + " " + webRequest.result + " " + webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.Success:
                    string output = webRequest.downloadHandler.text;
                    BroadcastResponse response = JsonConvert.DeserializeObject<BroadcastResponse>(output);
                    OnSigningTransactionStatusChanged(OperationStatus.Complete, response.txHash);
                    break;
            }
        }
        #endregion


        #region CheckTransactionStatus
        internal void CheckTransactionStatus(string txHash, UnityAction<OperationStatus, string> transactionStatus, float delay)
        {
            OnBlockchainTransactionStatusChanged = transactionStatus;
            string url = Constants.transactionStatusAPI;
            url = url.Replace("{txHash}", txHash);
            StartCoroutine(GetTransactionStatusRequest(url, delay));
        }


        private IEnumerator GetTransactionStatusRequest(string uri, float delay)
        {
            yield return new WaitForSeconds(delay);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        OnBlockchainTransactionStatusChanged(OperationStatus.Error, webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        OnBlockchainTransactionStatusChanged(OperationStatus.Error, webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        TransactionStatus status = JsonConvert.DeserializeObject<TransactionStatus>(webRequest.downloadHandler.text);
                        OnBlockchainTransactionStatusChanged(OperationStatus.Complete, status.status);
                        break;
                }
            }
        }
        #endregion


        //TODO This should be removed and only use events
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

        #region NFTs
        public void LoadWalletNFTs(UnityAction<OperationStatus, string, NFTMetadata[]> LoadNFTStatus)
        {
            Debug.Log("Load wallet nfts");
            StartCoroutine(GetWalletNFTs(connectedAccount.Address));
            this.LoadNFTStatus = LoadNFTStatus;
        }

        private IEnumerator GetWalletNFTs(string address)
        {
            string url = Constants.getNFTAPI;
            //address = "erd1n9mq20fu59hvlm4wnqup5gsq2qey7s8ev8yvaekwp5val5fvmnsqjl4ykf";
            url = url.Replace("{address}", address);

            Debug.Log(url);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                Debug.Log(webRequest.result);
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log(webRequest.downloadHandler.text);
                        List<NFTMetadata> allNfts = JsonConvert.DeserializeObject<List<NFTMetadata>>(webRequest.downloadHandler.text);
                        for (int i = allNfts.Count - 1; i >= 0; i--)
                        {
                            if (allNfts[i].type == "MetaESDT")
                            {
                                //Debug.LogWarning(allNfts[i].name + " " + allNfts[i].identifier);
                                allNfts.RemoveAt(i);
                            }
                        }
                        LoadNFTStatus(OperationStatus.Complete, "Success", allNfts.ToArray());
                        break;
                    default:
                        LoadNFTStatus(OperationStatus.Error, webRequest.error, null);
                        break;
                }
            }
        }


        internal void SendNFT(string destinationAddress, string tokenIdentifier, int nonce, int quantity, UnityAction<OperationStatus, string> transactionStatus)
        {
            Debug.Log("tokenIdentifier" + tokenIdentifier + " nonce " + nonce);
            string hexaNonce = nonce.ToString("X");
            if (hexaNonce.Length % 2 == 1)
            {
                hexaNonce = "0" + hexaNonce;
            }

            string hexaQuantity = quantity.ToString("X");
            if (hexaQuantity.Length % 2 == 1)
            {
                hexaQuantity = "0" + hexaQuantity;
            }

            Debug.Log(destinationAddress);
            Erdcsharp.Domain.Address destination = Erdcsharp.Domain.Address.FromBech32("erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf");

            Debug.Log(destination.Hex);

            string data = "ESDTNFTTransfer" +
                            "@" + Erdcsharp.Domain.Helper.Converter.ToHexString(tokenIdentifier) +
                            "@" + hexaNonce +
                            "@" + hexaQuantity +
                            "@" + destination.Hex;

            long nrOfBytes = System.Text.ASCIIEncoding.Unicode.GetByteCount(data);

            Debug.Log(data);

            SendTransaction(connectedAccount.Address, 0.ToString(), data, transactionStatus, 1000000 + nrOfBytes * 1500);
        }

        #endregion
    }
}