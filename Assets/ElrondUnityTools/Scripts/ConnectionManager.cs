using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models;
using Vector2 = UnityEngine.Vector2;

namespace ElrondUnityTools
{
    public class ConnectionManager : WalletConnectActions
    {
        private AccountDto connectedAccount;
        private IElrondProvider provider;
        private NetworkConfig networkConfig;
        private UnityAction<OperationStatus, string> OnSigningTransactionStatusChanged;
        private UnityAction<OperationStatus, string> OnBlockchainTransactionStatusChanged;
        private UnityAction<AccountDto> OnWalletConnected;
        private UnityAction OnWalletDisconnected;
        private WalletConnect walletConnect;
        private bool walletConnected;

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


        internal async void Connect(UnityAction<AccountDto> OnWalletConnected, UnityAction OnWalletDisconnected, Image qrImage)
        {
            if (gameObject.GetComponent<WalletConnect>())
            {
                AddQRImageScript(qrImage);
                return;
            }

            this.OnWalletConnected = OnWalletConnected;
            this.OnWalletDisconnected = OnWalletDisconnected;
            walletConnect = gameObject.AddComponent<WalletConnect>();
            walletConnect.connectOnStart = false;
            walletConnect.connectOnAwake = false;
            walletConnect.createNewSessionOnSessionDisconnect = false;
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
            walletConnect.ConnectedEvent.AddListener(Connected);
            AddQRImageScript(qrImage);

            provider = new ElrondProviderUnity(new ElrondNetworkConfiguration(Constants.networkType));
            networkConfig = await NetworkConfig.GetFromNetwork(provider);
        }


        async void AddQRImageScript(Image qrImage)
        {
            if (qrImage != null)
            {
                qrImage.gameObject.AddComponent<WalletConnectQRImage>().Init(walletConnect);
            }

            await walletConnect.Connect();
        }

        private void Connected()
        {
            OnConnected();
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
            WalletConnect.Instance.CloseSession(false);
        }


        #region SendTransaction
        internal void SendESDTTransaction(string destinationAddress, string amount, ESDTToken token, UnityAction<OperationStatus, string> transactionStatus)
        {

            //string[] array = amount.Split(new char[1] { '.' });
            //string obj = array.FirstOrDefault() ?? "0";
            //string text = ((array.Length == 2) ? array[1] : string.Empty);
            //string something = obj + text.PadRight(token.decimals, '0');
            //BigInteger value = BigInteger.Parse(something);
            //if (value.Sign == -1)
            //{
            //    throw new InvalidTokenAmountException(something);
            //}
            //Debug.Log(something+" "+value);
            amount = amount.Replace(",", ".");
            string hexaAmount = TokenAmount.ESDT(amount, token.ToToken()).Value.ToString("X");
            if (hexaAmount.Length % 2 == 1)
            {
                hexaAmount = "0" + hexaAmount;
            }


            string hexaTokenIdentifier = Erdcsharp.Domain.Helper.Converter.ToHexString(token.identifier);
            string data = "ESDTTransfer" +
                "@" + hexaTokenIdentifier +
                "@" + hexaAmount;

            //https://docs.elrond.com/tokens/esdt-tokens/
            //the GasLimit must be set to the value required by the protocol for ESDT transfers, namely 500000
            long gas = 500000;

            SendTransaction(destinationAddress, 0.ToString(), data, transactionStatus, gas);
        }

        internal void SendEGLDTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> TransactionStatus)
        {
            long gas = networkConfig.MinGasLimit + System.Text.ASCIIEncoding.Unicode.GetByteCount(data) * networkConfig.GasPerDataByte;
            SendTransaction(destinationAddress, amount, data, TransactionStatus, gas);
        }


        internal async void SendTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> transactionStatus, long gasLimit)
        {
            amount = amount.Replace(",", ".");
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



        private void OnConnected()
        {
            walletConnected = true;
            WalletConnect.ActiveSession.OnSessionDisconnect += ActiveSessionOnDisconnect;
            RefreshAccount(AccountRefreshed);

        }

        private void AccountRefreshed()
        {
            OnWalletConnected(connectedAccount);
        }

        public async void RefreshAccount(UnityAction CompleteMethod)
        {
            connectedAccount = await provider.GetAccount(WalletConnect.ActiveSession.Accounts[0]);
            if (CompleteMethod != null)
            {
                CompleteMethod();
            }
        }


        private void ActiveSessionOnDisconnect(object sender, EventArgs e)
        {
            WalletConnect.ActiveSession.OnSessionDisconnect -= ActiveSessionOnDisconnect;
            walletConnected = false;
            OnWalletDisconnected();
        }

        #region Tokens
        internal void LoadAllTokens(UnityAction<OperationStatus, string, TokenMetadata[]> loadTokensComplete)
        {
            StartCoroutine(GetWalletTokens(connectedAccount.Address, loadTokensComplete));
        }

        private IEnumerator GetWalletTokens(string address, UnityAction<OperationStatus, string, TokenMetadata[]> loadTokensComplete)
        {
            string url = Constants.getTokensCount;
            url = url.Replace("{address}", address);

            int totalTokens = 0;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        totalTokens = int.Parse(webRequest.downloadHandler.text);
                        break;
                    default:
                        loadTokensComplete(OperationStatus.Error, webRequest.error, null);
                        break;
                }
            }
            if (totalTokens == 0)
                yield break;

            url = Constants.getTokensAPI;
            url = url.Replace("{address}", address);
            url += "?from=0&size=" + totalTokens;

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        try
                        {
                            List<TokenMetadata> allNfts = JsonConvert.DeserializeObject<List<TokenMetadata>>(webRequest.downloadHandler.text);
                            loadTokensComplete(OperationStatus.Complete, "Success", allNfts.ToArray());
                        }
                        catch (Exception e)
                        {
                            loadTokensComplete(OperationStatus.Error, e.Message + ": " + e.Data, null);
                        }
                        break;
                    default:
                        loadTokensComplete(OperationStatus.Error, webRequest.error, null);
                        break;
                }
            }
        }
        #endregion

        #region NFTs
        public void LoadWalletNFTs(UnityAction<OperationStatus, string, NFTMetadata[]> LoadNFTCompletes)
        {
            StartCoroutine(GetWalletNFTs(connectedAccount.Address, LoadNFTCompletes));
        }


        private IEnumerator GetWalletNFTs(string address, UnityAction<OperationStatus, string, NFTMetadata[]> LoadNFTsComplete)
        {
            string url = Constants.getNFTCount;
            url = url.Replace("{address}", address);

            int totalNfts = 0;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        totalNfts = int.Parse(webRequest.downloadHandler.text);
                        break;
                    default:
                        LoadNFTsComplete(OperationStatus.Error, webRequest.error, null);
                        break;
                }
            }
            if (totalNfts == 0)
                yield break;

            url = Constants.getNFTAPI;
            url = url.Replace("{address}", address);
            url += "?from=" + 0 + "&size=" + totalNfts;

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        List<NFTMetadata> allNfts = JsonConvert.DeserializeObject<List<NFTMetadata>>(webRequest.downloadHandler.text);
                        for (int i = allNfts.Count - 1; i >= 0; i--)
                        {
                            //remove the LP tokens
                            if (allNfts[i].type == "MetaESDT")
                            {
                                allNfts.RemoveAt(i);
                            }
                        }
                        LoadNFTsComplete(OperationStatus.Complete, "Success", allNfts.ToArray());
                        break;
                    default:
                        LoadNFTsComplete(OperationStatus.Error, webRequest.error, null);
                        break;
                }
            }
        }


        internal void SendNFT(string destinationAddress, string collectionIdentifier, int nonce, int quantity, UnityAction<OperationStatus, string> transactionStatus)
        {
            string hexaNonce = nonce.ToString("X");
            if (hexaNonce.Length % 2 == 1)
            {
                hexaNonce = "0" + hexaNonce;
            }

            string hexQuantity = quantity.ToString("X");
            if (hexQuantity.Length % 2 == 1)
            {
                hexQuantity = "0" + hexQuantity;
            }

            Erdcsharp.Domain.Address destination = Erdcsharp.Domain.Address.FromBech32(destinationAddress);

            string data = "ESDTNFTTransfer" +
                            "@" + Erdcsharp.Domain.Helper.Converter.ToHexString(collectionIdentifier) +
                            "@" + hexaNonce +
                            "@" + hexQuantity +
                            "@" + destination.Hex;

            long nrOfBytes = System.Text.ASCIIEncoding.Unicode.GetByteCount(data);

            //https://docs.elrond.com/tokens/nft-tokens/#tab-group-43-content-44
            long gas = 1000000 + nrOfBytes * networkConfig.GasPerDataByte;

            SendTransaction(connectedAccount.Address, 0.ToString(), data, transactionStatus, gas);
        }
        #endregion


        #region SCs
        internal void MakeSCQuery(string scAddress, string methodName, string[] args, UnityAction<OperationStatus, string, SCData> QueryComplete)
        {
            StartCoroutine(PostSCQuery(Constants.scQueryAPI, new SCQuery(scAddress, methodName, args), QueryComplete));
        }

        IEnumerator PostSCQuery(string uri, SCQuery query, UnityAction<OperationStatus, string, SCData> QueryComplete)
        {
            string json = JsonUtility.ToJson(query);

            using var webRequest = new UnityWebRequest();
            webRequest.url = uri;
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "*/*");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    string output = webRequest.downloadHandler.text;
                    QueryResponse response;

                    try
                    {
                        response = JsonConvert.DeserializeObject<QueryResponse>(output);
                    }
                    catch (Exception e)
                    {
                        QueryComplete(OperationStatus.Error, "Deserialization error: " + e.Message + " " + e.Data, null);
                        break;
                    }


                    if (!string.IsNullOrEmpty(response.error))
                    {
                        QueryComplete(OperationStatus.Error, "SC Call error: " + response.error + " " + response.code, null);
                        break;
                    }

                    SCData data = response.data.data;
                    if (data.returnData == null)
                    {
                        QueryComplete(OperationStatus.Error, "Data error: " + data.returnCode + " " + data.returnMessage, null);
                        break;
                    }

                    QueryComplete(OperationStatus.Complete, response.code, data);
                    break;

                default:
                    QueryComplete(OperationStatus.Error, webRequest.error, null);
                    break;
            }

        }


        internal void CallSCMethod(string scAddress, string methodName, long gasRequiredForSCExecution, UnityAction<OperationStatus, string> QueryComplete, params object[] args)
        {
            string data = methodName;
            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log(args[i].GetType());
                string converted;
                if (args[i].IsNumericType())
                {
                    converted = args[i].ToHex();
                    if (converted.Length % 2 == 1)
                    {
                        converted = "0" + converted;
                    }
                    data += "@" + converted;
                }
                else
                {
                    converted = (string)args[i];
                    data += "@" + Erdcsharp.Domain.Helper.Converter.ToHexString(converted);
                }
            }

            long nrOfBytes = System.Text.ASCIIEncoding.Unicode.GetByteCount(data);
            long gas = gasRequiredForSCExecution + nrOfBytes * networkConfig.GasPerDataByte;

            SendTransaction(scAddress, 0.ToString(), data, QueryComplete, gas);
        }
        #endregion

        #region GenericMethods
        internal void GetRequest(string url, UnityAction<OperationStatus, string, string> completeMethod)
        {
            StartCoroutine(MakeAPICall(url, completeMethod));
        }


        private IEnumerator MakeAPICall(string url, UnityAction<OperationStatus, string, string> completeMethod)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        completeMethod(OperationStatus.Complete, "Success", webRequest.downloadHandler.text);
                        break;
                    default:
                        completeMethod(OperationStatus.Error, webRequest.error, null);
                        break;
                }
            }
        }


        internal void PostRequest(string url, string jsonData, UnityAction<OperationStatus, string, string> completeMethod)
        {
            StartCoroutine(Post(url, jsonData, completeMethod));
        }


        IEnumerator Post(string url, string jsonData, UnityAction<OperationStatus, string, string> completeMethod)
        {
            using var webRequest = new UnityWebRequest();
            webRequest.url = url;
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    completeMethod(OperationStatus.Complete, "Success", webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    completeMethod(OperationStatus.Error, webRequest.error, webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    completeMethod(OperationStatus.Error, webRequest.error + " " + webRequest.result, webRequest.downloadHandler.text);
                    break;
            }
        }

        private void OnDestroy()
        {
            walletConnect.ConnectedEvent.RemoveAllListeners();
        }
        #endregion

        #region Utils

        public void LoadImage(string imageURL, Image displayComponent, UnityAction<OperationStatus, string> completeMethod)
        {
            StartCoroutine(LoadImageCoroutine(imageURL, displayComponent, completeMethod));
        }

        /// <summary>
        /// Load the NFT Thumbnail from the url
        /// </summary>
        /// <param name="imageURL"></param>
        /// <param name="displayComponent">image component to display the downloaded thumbnail picture</param>
        /// <returns></returns>
        private IEnumerator LoadImageCoroutine(string imageURL, Image displayComponent, UnityAction<OperationStatus, string> completeMethod)
        {
            if(!string.IsNullOrEmpty(Constants.CORSFixUrl))
            {
                imageURL = Constants.CORSFixUrl + UnityWebRequest.EscapeURL(imageURL);
            }

            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageURL);
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (displayComponent)
                    {
                        Texture2D imageTex = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                        Sprite newSprite = Sprite.Create(imageTex, new Rect(0, 0, imageTex.width, imageTex.height), new Vector2(.5f, .5f));
                        displayComponent.sprite = newSprite;
                        if (completeMethod != null)
                        {
                            completeMethod(OperationStatus.Complete, "Success");
                        }
                    }
                    break;
                default:
                    if (completeMethod != null)
                    {
                        completeMethod(OperationStatus.Error, webRequest.error);
                    }
                    break;
            }
        }

        #endregion
    }
}


