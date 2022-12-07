using Erdcsharp.Domain;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models;


namespace MultiversXUnityTools
{
    public class ConnectionManager : MonoBehaviour
    {
        private Account connectedAccount;
        private NetworkConfig networkConfig;
        private UnityAction<Account> OnWalletConnected;
        private UnityAction OnWalletDisconnected;
        private WalletConnect walletConnect;
        private BinaryCodec BinaryCoder = new BinaryCodec();
        private APISettings apiSettings;
        private API selectedAPI;
        private IMultiversXApiProvider multiversXProvider;
        private bool walletConnected;


        //Create a static instance for easy access
        private static ConnectionManager instance;
        internal static ConnectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject(Constants.CONNECTION_MANAGER_OBJECT);
                    instance = go.AddComponent<ConnectionManager>();

                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }


        #region Connect
        /// <summary>
        /// Connect to wallet connect service to obtain the keys
        /// </summary>
        /// <param name="OnWalletConnected">connected callback</param>
        /// <param name="OnWalletDisconnected">disconnected callback</param>
        /// <param name="qrImage">image to display the QRcode if required</param>
        internal async void Connect(UnityAction<Account> OnWalletConnected, UnityAction OnWalletDisconnected, Image qrImage)
        {
            //if already connected do not connect
            if (gameObject.GetComponent<WalletConnect>())
            {
                AddQRImageScript(qrImage);
                await LoadAPI();
                return;
            }

            //setup events and parameters
            this.OnWalletConnected = OnWalletConnected;
            this.OnWalletDisconnected = OnWalletDisconnected;
            walletConnect = gameObject.AddComponent<WalletConnect>();
            walletConnect.connectOnStart = false;
            walletConnect.connectOnAwake = false;
            walletConnect.createNewSessionOnSessionDisconnect = false;
            apiSettings = Manager.GetApiSettings();
            ClientMeta appData = new ClientMeta();
            appData.Description = apiSettings.appDescription;
            appData.Icons = new string[1];
            appData.Icons[0] = apiSettings.appIcon;
            appData.Name = apiSettings.appName;
            appData.URL = apiSettings.appWebsite;
            walletConnect.AppData = appData;
            walletConnect.customBridgeUrl = Constants.CUSTOM_BRIDGE_URL;
            walletConnect.ConnectedEvent = new WalletConnect.WalletConnectEventNoSession();
            walletConnect.ConnectedEventSession = new WalletConnect.WalletConnectEventWithSessionData();
            walletConnect.ConnectedEvent.AddListener(Connected);
            AddQRImageScript(qrImage);
            await LoadAPI();
        }


        /// <summary>
        /// Load required API json file
        /// </summary>
        /// <returns></returns>
        async Task LoadAPI()
        {
            if (apiSettings == null || string.IsNullOrEmpty(apiSettings.selectedAPIName))
            {
                Debug.LogError("No API settings file found -> Go to ... and generate one");
                return;
            }

           
            TextAsset targetFile = Resources.Load<TextAsset>($"APIs/{apiSettings.selectedAPIName}");
            try
            {
                selectedAPI = JsonConvert.DeserializeObject<API>(targetFile.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message} {e.Data}");
                return;
            }

            if (selectedAPI == null)
            {
                Debug.LogError("Null API selected");
                return;
            }

            Debug.Log($"SELECTED API { selectedAPI.apiName} {selectedAPI.baseAddress}");
            multiversXProvider = new ElrondProviderUnity(selectedAPI);
            networkConfig = await LoadNetworkConfig(false, true);
        }


        /// <summary>
        /// Load blockchain configuration data
        /// </summary>
        /// <param name="throwException">if true, throws exception if fails</param>
        /// <param name="forceReload">force to load config when API changes</param>
        /// <returns>config params</returns>
        private async Task<NetworkConfig> LoadNetworkConfig(bool throwException = false, bool forceReload = false)
        {
            if (networkConfig != null && forceReload == false)
            {
                return networkConfig;
            }
            try
            {
                networkConfig = await NetworkConfig.GetFromNetwork(multiversXProvider);
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


        /// <summary>
        /// Add QR code to the image component and connect 
        /// </summary>
        /// <param name="qrImage"></param>
        private async void AddQRImageScript(Image qrImage)
        {
            if (qrImage != null)
            {
                qrImage.gameObject.AddComponent<WalletConnectQRImage>().Init(walletConnect);
            }

            await walletConnect.Connect();
        }


        /// <summary>
        /// Called when connection is established
        /// </summary>
        private void Connected()
        {
            walletConnected = true;
            WalletConnect.ActiveSession.OnSessionDisconnect += ActiveSessionOnDisconnect;
            walletConnect.ConnectedEvent.RemoveListener(Connected);
            //After wallet is connected get the account information automatically
            RefreshAccount(AccountRefreshed);
        }


        /// <summary>
        /// Listener for the wallet disconnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveSessionOnDisconnect(object sender, EventArgs e)
        {
            WalletConnect.ActiveSession.OnSessionDisconnect -= ActiveSessionOnDisconnect;
            walletConnected = false;
            //trigger the wallet disconnect callback from Connect
            OnWalletDisconnected();
            Destroy(walletConnect.gameObject);
        }


        /// <summary>
        /// Save the connected account and get the wallet information
        /// </summary>
        /// <param name="completeMethod"></param>
        public async void RefreshAccount(UnityAction<OperationStatus, string> completeMethod)
        {
            connectedAccount = new Account(Erdcsharp.Domain.Address.From(WalletConnect.ActiveSession.Accounts[0]));
            try
            {
                await connectedAccount.Sync(multiversXProvider);
                completeMethod?.Invoke(OperationStatus.Complete, "Success");
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
            }
        }


        /// <summary>
        /// Callback for wallet refresh complete
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        private void AccountRefreshed(OperationStatus status, string message)
        {
            if (status == OperationStatus.Complete)
            {
                //trigger the on connected trigger from the Connect method
                OnWalletConnected(connectedAccount);
            }
            else
            {
                Debug.LogError($"{status} {message}");
            }
        }
        #endregion


        #region SendTransaction
        /// <summary>
        /// Send EGLD to another address
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <param name="data"></param>
        /// <param name="completeMethod"></param>
        internal void SendEGLDTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> completeMethod)
        {
            SendTransaction(destinationAddress, amount, data, completeMethod, 0);
        }


        /// <summary>
        /// Actually constructs the transaction, send it for signing, and broadcast the signed transaction to the blockchain
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <param name="data"></param>
        /// <param name="completeMethod"></param>
        /// <param name="requiredGas"></param>
        private async void SendTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> completeMethod, long requiredGas)
        {
            //verify the parameters first

            //make sure the amount value is correct
            OperationResult result = Utilities.IsNumberValid(ref amount);
            if (result.status == OperationStatus.Error)
            {
                completeMethod?.Invoke(result.status, result.message);
                return;
            }

            //verify the address
            if (!Utilities.IsAddressValid(destinationAddress))
            {
                completeMethod?.Invoke(OperationStatus.Error, "Invalid destination address");
                return;
            }


            //check network config params
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

            //refresh account nonce
            try
            {
                await connectedAccount.Sync(multiversXProvider);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }


            //check EGLD balance
            if (TokenAmount.EGLD(amount).Value > connectedAccount.Balance.Value)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"Insufficient funds, required : {TokenAmount.EGLD(amount).ToDenominated()} and got {connectedAccount.Balance.ToDenominated()}");
                return;
            }

            //compute gas
            requiredGas += networkConfig.MinGasLimit + System.Text.Encoding.ASCII.GetBytes(data).Length * networkConfig.GasPerDataByte;

            //construct the transaction
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

            //trigger a partial callback
            completeMethod?.Invoke(OperationStatus.InProgress, "Waiting for signing");

            //wait for the signature from wallet
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

            //apply the signature and broadcast the transaction
            TransactionRequestDto signedTransaction = transaction.ToSignedTransaction(signature);
            try
            {
                //send the transaction hash inside complete method
                var response = await multiversXProvider.SendTransaction(signedTransaction);
                completeMethod?.Invoke(OperationStatus.Complete, response.TxHash);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }
        }


        /// <summary>
        /// Sign a single MultiversX transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal async Task<string> SignTransaction(TransactionData transaction)
        {
            walletConnect.OpenMobileWallet();
            var results = await WalletConnect.ActiveSession.ErdSignTransaction(transaction);

            return results;
        }


        /// <summary>
        /// Sign multiple MultiversX transactions
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal async Task<string> BatchSignTransaction(TransactionData transaction)
        {
            walletConnect.OpenMobileWallet();
            var results = await WalletConnect.ActiveSession.ErdBatchSignTransaction(transaction);

            return results;
        }
        #endregion


        #region CheckTransactionStatus
        /// <summary>
        /// Check if a broadcasted transaction was executed and if it was successful
        /// </summary>
        /// <param name="txHash">the hash of the transaction</param>
        /// <param name="completeMethod">callback method</param>
        /// <param name="refreshTime">time interval to query the blockchain for the status of the transaction. Lower times means more calls to blockchain APIs</param>
        internal void CheckTransactionStatus(string txHash, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            MultiversXTransaction tx = new MultiversXTransaction(txHash);
            Sync(tx, completeMethod, refreshTime);
            return;

            //old implementation, does not work on WebGL builds
            //string message;
            //try
            //{
            //    await tx.AwaitExecuted(multiversXProvider);
            //    if (!tx.EnsureTransactionSuccess(out message))
            //    {
            //        completeMethod?.Invoke(OperationStatus.Error, message);
            //        return;
            //    }
            //}
            //catch (Exception e)
            //{
            //    completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
            //    return;
            //}

            //completeMethod?.Invoke(OperationStatus.Complete, tx.Status);
        }


        /// <summary>
        /// Start to sync a transaction
        /// </summary>
        /// <param name="tx">transaction</param>
        /// <param name="completeMethod">callback when completed</param>
        /// <param name="refreshTime"></param>
        void Sync(MultiversXTransaction tx, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            StartCoroutine(CheckTransaction(tx, completeMethod,refreshTime));
        }


        /// <summary>
        /// Coroutine to wait before chacking the status
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="completeMethod"></param>
        /// <param name="refreshTime"></param>
        /// <returns></returns>
        private IEnumerator CheckTransaction(MultiversXTransaction tx, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            yield return new WaitForSeconds(refreshTime);
            SyncTransaction(tx, completeMethod,refreshTime);
        }


        /// <summary>
        /// Check if the current transaction is executed or try again if not
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="completeMethod"></param>
        /// <param name="refreshTime"></param>
        private async void SyncTransaction(MultiversXTransaction tx, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            await tx.Sync(multiversXProvider);
            if (tx.IsExecuted())
            {
                string message;
                if (!tx.EnsureTransactionSuccess(out message))
                {
                    completeMethod?.Invoke(OperationStatus.Error, message);
                }
                else
                {
                    completeMethod?.Invoke(OperationStatus.Complete, tx.Status);
                }
                return;
            }
            Sync(tx, completeMethod, refreshTime);
        }
        #endregion


        #region Tokens
        /// <summary>
        /// Load all tokens from a wallet
        /// </summary>
        /// <param name="completeMethod"></param>
        internal async void LoadAllTokens(UnityAction<OperationStatus, string, TokenMetadata[]> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(OperationStatus.Complete, "Success", await multiversXProvider.GetWalletTokens<TokenMetadata[]>(connectedAccount.Address.ToString()));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, e.Message + ": " + e.Data, null);
            }
        }


        /// <summary>
        /// Send any ESDT token to another address
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <param name="token"></param>
        /// <param name="completeMethod"></param>
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
        #endregion


        #region NFTs
        /// <summary>
        /// Load all NFTs from a wallet
        /// </summary>
        /// <param name="completeMethod"></param>
        public async void LoadWalletNFTs(UnityAction<OperationStatus, string, NFTMetadata[]> completeMethod)
        {
            try
            {
                List<NFTMetadata> allNfts = await multiversXProvider.GetWalletNfts<List<NFTMetadata>>(connectedAccount.Address.ToString());
                for (int i = allNfts.Count - 1; i >= 0; i--)
                {
                    //remove the LP tokens
                    if (allNfts[i].type == "MetaESDT")
                    {
                        allNfts.RemoveAt(i);
                    }
                }
                completeMethod?.Invoke(OperationStatus.Complete, "Success", allNfts.ToArray());
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", null);
                return;
            }
        }


        /// <summary>
        /// Send any NFT token to another address
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="collectionIdentifier"></param>
        /// <param name="nonce"></param>
        /// <param name="quantity"></param>
        /// <param name="completeMethod"></param>
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
        /// <summary>
        /// Make a query (check stored values)
        /// </summary>
        /// <typeparam name="T">type of the expected result</typeparam>
        /// <param name="scAddress">the address of the Smart Contract</param>
        /// <param name="methodName">the method to call</param>
        /// <param name="completeMethod">callback method</param>
        /// <param name="outputType">typevalue of the output</param>
        /// <param name="args">list of arguments if required</param>
        internal async void MakeSCQuery<T>(string scAddress, string methodName, UnityAction<OperationStatus, string, T> completeMethod, TypeValue outputType, params IBinaryType[] args) where T : IBinaryType
        {
            var queryResult = await SmartContract.QuerySmartContract<T>(multiversXProvider, Erdcsharp.Domain.Address.From(scAddress), outputType, methodName, connectedAccount.Address, args);
            if (string.IsNullOrEmpty(queryResult.Item2))
            {
                completeMethod(OperationStatus.Complete, "Success", queryResult.Item1);
            }
            else
            {
                completeMethod?.Invoke(OperationStatus.Error, queryResult.Item2, default(T));
            }
        }


        /// <summary>
        /// Perform a Smart Contract call 
        /// </summary>
        /// <param name="scAddress"></param>
        /// <param name="methodName"></param>
        /// <param name="gasRequiredForSCExecution"></param>
        /// <param name="completeMethod"></param>
        /// <param name="args"></param>
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
        /// <summary>
        /// Make any type of GET request using the API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="completeMethod"></param>
        internal async void GetRequest<T>(string url, UnityAction<OperationStatus, string, T> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(OperationStatus.Complete, "Success", await multiversXProvider.GetRequest<T>(url));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", default(T));
            }
        }


        /// <summary>
        /// Make any kind of POST request using the API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="jsonData"></param>
        /// <param name="completeMethod"></param>
        internal async void PostRequest<T>(string url, string jsonData, UnityAction<OperationStatus, string, T> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(OperationStatus.Complete, "Success", await multiversXProvider.PostRequest<T>(url, jsonData));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", default(T));
            }
        }
        #endregion


        #region Utils
        /// <summary>
        /// Load an image and display it automatically on displayComponent when loaded
        /// </summary>
        /// <param name="imageURL"></param>
        /// <param name="displayComponent"></param>
        /// <param name="completeMethod"></param>
        internal void LoadImage(string imageURL, Image displayComponent, UnityAction<OperationStatus, string> completeMethod)
        {
            StartCoroutine(LoadImageCoroutine(imageURL, displayComponent, completeMethod));
        }

        /// <summary>
        /// Load an image from the url
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


        #region PublicMethods
        /// <summary>
        /// Automatically open the Maiar wallet on mobile devices
        /// </summary>
        internal void DeepLinkLogin()
        {
            walletConnect.OpenDeepLink();
        }


        /// <summary>
        /// Check if wallet is connected 
        /// </summary>
        /// <returns></returns>
        internal bool IsWalletConnected()
        {
            return walletConnected;
        }


        /// <summary>
        /// Returns the connected account
        /// </summary>
        /// <returns></returns>
        internal Account GetConnectedAccount()
        {
            return connectedAccount;
        }


        /// <summary>
        /// Disconnect the wallet
        /// </summary>
        internal void Disconnect()
        {
            if (WalletConnect.ActiveSession == null || !WalletConnect.ActiveSession.Connected)
            {
                Destroy(this.gameObject);
                return;
            }
            walletConnect.CloseSession(false);
        }


        /// <summary>
        /// Returns the current selected MultiversX API
        /// </summary>
        /// <returns></returns>
        internal APISettings GetApiSettings()
        {
            if (apiSettings == null)
            {
                apiSettings = Resources.Load<APISettings>(Constants.API_SETTINGS_DATA);
            }
            return apiSettings;
        }


        /// <summary>
        /// Returns the complete url based on the current selected API
        /// </summary>
        /// <param name="endpoint">Name of the endpoint from the Settings Window</param>
        /// <returns></returns>
        internal string GetEndpointUrl(EndpointNames endpoint)
        {
            if (selectedAPI == null)
            {
                Debug.LogError("Call connect method first");
                return null;
            }

            return selectedAPI.GetEndpoint(endpoint);
        }
        #endregion


        private void OnDestroy()
        {
            if (walletConnect)
            {
                walletConnect.ConnectedEvent.RemoveAllListeners();
            }
        }
    }
}


