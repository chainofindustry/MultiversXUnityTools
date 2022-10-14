using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models;

namespace ElrondUnityTools
{
    public class ConnectionManager : WalletConnectActions
    {
        private Account connectedAccount;
        private IElrondProvider elrondAPI;
        private NetworkConfig networkConfig;
        private UnityAction<Account> OnWalletConnected;
        private UnityAction OnWalletDisconnected;
        private WalletConnect walletConnect;
        private bool walletConnected;
        BinaryCodec BinaryCoder = new BinaryCodec();

        private static ConnectionManager instance;
        internal static ConnectionManager Instance
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


        internal async void Connect(UnityAction<Account> OnWalletConnected, UnityAction OnWalletDisconnected, Image qrImage)
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

            elrondAPI = new ElrondProviderUnity(new ElrondNetworkConfiguration(Constants.networkType));
            //elrondAPI = new ElrondProvider(new System.Net.Http.HttpClient(), new ElrondNetworkConfiguration(Constants.networkType));
            networkConfig = await LoadNetworkConfig();
        }

        private async Task<NetworkConfig> LoadNetworkConfig(bool throwException = false)
        {
            if (networkConfig != null)
            {
                return networkConfig;
            }
            try
            {
                networkConfig = await NetworkConfig.GetFromNetwork(elrondAPI);
                return networkConfig;
            }
            catch (Exception e)
            {
                if (throwException)
                {
                    throw e;
                }
            }
            return null;
        }

        private async void AddQRImageScript(Image qrImage)
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
            walletConnect.CloseSession(false);
        }


        #region SendTransaction
        internal void SendESDTTransaction(string destinationAddress, string amount, Token token, UnityAction<OperationStatus, string> completeMethod)
        {
            OperationResult result = Utilities.IsNumberValid(ref amount);
            if (result.status == OperationStatus.Error)
            {
                completeMethod?.Invoke(result.status, result.message);
                return;
            }

            //https://docs.elrond.com/tokens/esdt-tokens/
            //the GasLimit must be set to the value required by the protocol for ESDT transfers, namely 500000
            long gas = 500000;

            CallSCMethod(destinationAddress, "ESDTTransfer", gas, completeMethod, TokenIdentifierValue.From(token.Ticker), NumericValue.TokenAmount(TokenAmount.ESDT(amount, token)));
        }

        internal void SendEGLDTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> completeMethod)
        {
            SendTransaction(destinationAddress, amount, data, completeMethod, 0);
        }


        private async void SendTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> completeMethod, long requiredGas)
        {
            //make sure the amount value is correct
            OperationResult result = Utilities.IsNumberValid(ref amount);
            if (result.status == OperationStatus.Error)
            {
                completeMethod?.Invoke(result.status, result.message);
                return;
            }

            if (!Utilities.IsAddressValid(destinationAddress))
            {
                completeMethod?.Invoke(OperationStatus.Error, "Invalid destination address");
                return;
            }

            NetworkConfig networkConfig = null;
            try
            {
                networkConfig = await LoadNetworkConfig(true);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }

            //refresh account
            try
            {
                await connectedAccount.Sync(elrondAPI);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }

            if (TokenAmount.EGLD(amount).Value > connectedAccount.Balance.Value)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"Insufficient funds, required : {TokenAmount.EGLD(amount).ToDenominated()} and got {connectedAccount.Balance.ToDenominated()}");
                return;
            }

            requiredGas += networkConfig.MinGasLimit + Convert.FromBase64String(data).Length * networkConfig.GasPerDataByte;

            var transaction = new TransactionData()
            {
                nonce = connectedAccount.Nonce,
                from = connectedAccount.Address.ToString(),
                to = destinationAddress,
                amount = TokenAmount.EGLD(amount).ToString(),
                data = data,
                gasPrice = networkConfig.MinGasPrice.ToString(),
                gasLimit = requiredGas.ToString(),
                chainId = networkConfig.ChainId,
                version = networkConfig.MinTransactionVersion
            };

            completeMethod?.Invoke(OperationStatus.InProgress, "Waiting for signing");

            string signature;
            try
            {
                signature = await SignTransaction(transaction);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }


            TransactionRequestDto signedTransaction = transaction.ToSignedTransaction(signature);
            try
            {
                var response = await elrondAPI.SendTransaction(signedTransaction);
                completeMethod?.Invoke(OperationStatus.Complete, response.TxHash);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }
        }
        #endregion


        #region CheckTransactionStatus
        internal async void CheckTransactionStatus(string txHash, UnityAction<OperationStatus, string> completeMethod)
        {
            Transaction tx = new Transaction(txHash);
            try
            {
                await tx.AwaitExecuted(elrondAPI);
                tx.EnsureTransactionSuccess();
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }

            completeMethod?.Invoke(OperationStatus.Complete, tx.Status);
        }
        #endregion



        private void OnConnected()
        {
            walletConnected = true;
            WalletConnect.ActiveSession.OnSessionDisconnect += ActiveSessionOnDisconnect;
            RefreshAccount(AccountRefreshed);

        }

        private void AccountRefreshed(OperationStatus status, string message)
        {
            OnWalletConnected(connectedAccount);
        }

        public async void RefreshAccount(UnityAction<OperationStatus, string> completeMethod)
        {
            connectedAccount = new Account(Erdcsharp.Domain.Address.From(WalletConnect.ActiveSession.Accounts[0]));
            try
            {
                await connectedAccount.Sync(elrondAPI);
                completeMethod?.Invoke(OperationStatus.Complete, "Success");
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
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
            StartCoroutine(GetWalletTokens(connectedAccount.Address.ToString(), loadTokensComplete));
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
            StartCoroutine(GetWalletNFTs(connectedAccount.Address.ToString(), LoadNFTCompletes));
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


        internal void SendNFT(string destinationAddress, string collectionIdentifier, ulong nonce, int quantity, UnityAction<OperationStatus, string> completeMethod)
        {
            //https://docs.elrond.com/tokens/nft-tokens/#tab-group-43-content-44
            long gas = 1000000;

            CallSCMethod(connectedAccount.Address.ToString(),
                "ESDTNFTTransfer",
                gas,
                completeMethod,
                TokenIdentifierValue.From(collectionIdentifier),
                NumericValue.BigUintValue(nonce),
                NumericValue.BigUintValue(quantity),
                Erdcsharp.Domain.Address.From(destinationAddress)
                );
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


        internal void CallSCMethod(string scAddress, string methodName, long gasRequiredForSCExecution, UnityAction<OperationStatus, string> completeMethod, params IBinaryType[] args)
        {
            string data = methodName;
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(BinaryCoder.EncodeTopLevel(arg))}");
            }

            SendTransaction(scAddress, 0.ToString(), data, completeMethod, gasRequiredForSCExecution);
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
                        completeMethod?.Invoke(OperationStatus.Complete, "Success", webRequest.downloadHandler.text);
                        break;
                    default:
                        completeMethod?.Invoke(OperationStatus.Error, webRequest.error, null);
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
                    completeMethod?.Invoke(OperationStatus.Complete, "Success", webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    completeMethod?.Invoke(OperationStatus.Error, webRequest.error, webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    completeMethod?.Invoke(OperationStatus.Error, webRequest.error + " " + webRequest.result, webRequest.downloadHandler.text);
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
            if (!string.IsNullOrEmpty(Constants.CORSFixUrl))
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
                        completeMethod?.Invoke(OperationStatus.Complete, "Success");

                    }
                    break;
                default:
                    completeMethod?.Invoke(OperationStatus.Error, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
                    break;
            }
        }
        #endregion
    }
}


