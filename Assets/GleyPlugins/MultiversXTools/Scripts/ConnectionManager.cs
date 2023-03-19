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
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models.Pairing;
using static UnityEditor.Progress;

namespace MultiversXUnityTools
{
    public class ConnectionManager : MonoBehaviour
    {
        private Account connectedAccount;
        private NetworkConfig networkConfig;
        private BinaryCodec binaryCoder = new BinaryCodec();
        private APISettings apiSettings;
        private API selectedAPI;
        private WalletConnect walletConnect;
        private IMultiversXApiProvider multiversXProvider;
        private string account;


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
        internal async void Connect(UnityAction<Account, string> OnWalletConnected, UnityAction OnWalletDisconnected, UnityAction<string> OnSessionConnected, Image qrImage)
        {
            apiSettings = GetApiSettings();
            LoadAPI();
            try
            {
                networkConfig = await LoadNetworkConfig(true, false);

                if (walletConnect == null)
                {
                    walletConnect = gameObject.AddComponent<WalletConnect>();
                }

                var metadata = new Metadata()
                {
                    Description = apiSettings.appDescription,
                    Icons = new[] { apiSettings.appIcon },
                    Name = apiSettings.appName,
                    Url = apiSettings.appWebsite
                };

                account = await walletConnect.Connect(
                    metadata,
                    networkConfig.ChainId,
                    (uri) =>
                    {
                        OnSessionConnected?.Invoke(uri);
                        AddQRImageScript(qrImage, uri);
                    },
                    OnWalletDisconnected
                    );

                Debug.Log($"Connected account: {account}");

                connectedAccount = new Account(Address.From(account));
                OnWalletConnected?.Invoke(connectedAccount, null);
            }
            catch (Exception e)
            {
                OnWalletConnected?.Invoke(null, $"{e.Message} {e.Data}");
            }
        }

        /// <summary>
        /// Maiar deep link urls
        /// </summary>
        public void OpenDeepLink()
        {
            walletConnect.OpenDeepLink();
        }

        private void AddQRImageScript(Image qrImage, string uri)
        {
            if (qrImage != null)
            {
                qrImage.gameObject.AddComponent<WalletConnectQRImage>().Init(uri);
            }
        }


        /// <summary>
        /// Load required API json file
        /// </summary>
        /// <returns></returns>
        void LoadAPI()
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

            Debug.Log($"SELECTED API {selectedAPI.apiName} {selectedAPI.baseAddress}");
            multiversXProvider = new ElrondProviderUnity(selectedAPI);
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
        /// Save the connected account and get the wallet information
        /// </summary>
        /// <param name="completeMethod"></param>
        public async void RefreshAccount(UnityAction<OperationStatus, string> completeMethod)
        {
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
        #endregion


        #region SendTransaction
        /// <summary>
        /// Send EGLD to another address
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <param name="data"></param>
        /// <param name="completeMethod"></param>
        internal void SendEGLDTransaction(TransactionToSign transaction, UnityAction<OperationStatus, string, string[]> completeMethod)
        {

            SendTransaction(new TransactionProcessed[] { new TransactionProcessed(transaction) }, completeMethod);
        }


        /// <summary>
        /// Actually constructs the transaction, send it for signing, and broadcast the signed transaction to the blockchain
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <param name="data"></param>
        /// <param name="completeMethod"></param>
        /// <param name="completeMethod"></param>
        /// <param name="requiredGas"></param>
        private async void SendTransaction(TransactionProcessed[] transactionsToSign, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            Debug.Log("SendTransaction");
            //verify the parameters first

            TransactionData[] transactions = new TransactionData[transactionsToSign.Length];
            for (int i = 0; i < transactionsToSign.Length; i++)
            {
                //make sure the amount value is correct
                OperationResult result = Utilities.IsNumberValid(ref transactionsToSign[i].value);
                if (result.status == OperationStatus.Error)
                {
                    completeMethod?.Invoke(result.status, result.message, null);
                    return;
                }

                //verify the address
                if (!Utilities.IsAddressValid(transactionsToSign[i].destination))
                {
                    completeMethod?.Invoke(OperationStatus.Error, "Invalid destination address", null);
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
                    completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", null);
                    return;
                }

                //refresh account nonce
                try
                {
                    await connectedAccount.Sync(multiversXProvider);
                }
                catch (Exception e)
                {
                    completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", null);
                    return;
                }


                //check EGLD balance
                if (TokenAmount.EGLD(transactionsToSign[i].value).Value > connectedAccount.Balance.Value)
                {
                    completeMethod?.Invoke(OperationStatus.Error, $"Insufficient funds, required : {TokenAmount.EGLD(transactionsToSign[i].value).ToDenominated()} and got {connectedAccount.Balance.ToDenominated()}", null);
                    return;
                }

                //compute gas
                transactionsToSign[i].gasRequiredForSCExecution += networkConfig.MinGasLimit + System.Text.Encoding.ASCII.GetBytes(transactionsToSign[i].data).Length * networkConfig.GasPerDataByte;

                //construct the transaction
                var transaction = new TransactionData()
                {
                    nonce = connectedAccount.Nonce + i,
                    sender = connectedAccount.Address.ToString(),
                    receiver = transactionsToSign[i].destination,
                    value = TokenAmount.EGLD(transactionsToSign[i].value).ToString(),
                    data = Convert.ToBase64String(Encoding.UTF8.GetBytes(transactionsToSign[i].data)),
                    gasPrice = networkConfig.MinGasPrice,
                    gasLimit = transactionsToSign[i].gasRequiredForSCExecution,
                    chainID = networkConfig.ChainId,
                    version = networkConfig.MinTransactionVersion
                };
                transactions[i] = transaction;
            }

            //wait for the signature from wallet
            //try
            //{
            if (transactions.Length == 1)
            {
                string signature = await walletConnect.SignTransaction(transactions[0]);
                TransactionRequestDto signedTransaction = transactions[0].ToSignedTransaction(signature);
                var response = await multiversXProvider.SendTransaction(signedTransaction);
                completeMethod?.Invoke(OperationStatus.Complete, null, new string[] { response.TxHash });
            }
            else
            {
                Debug.Log(1);
                string[] signatures = await walletConnect.SignTransactions(transactions);
                Debug.Log(2);
                TransactionRequestDto[] signedTransactions = new TransactionRequestDto[signatures.Length];
                Debug.Log(3);
                for (int i = 0; i < signatures.Length; i++)
                {
                    signedTransactions[i] = transactions[i].ToSignedTransaction(signatures[i]);
                }
                Debug.Log(4);
                var response = await multiversXProvider.SendTransactions(signedTransactions);
                Debug.Log(5);

                string[] txHashes = new string[response.NumOfSentTxs];
                foreach (var item in response.TxsHashes)
                {
                    Debug.Log(string.Format("Key: {0}, Value: {1}", item.Key, item.Value));
                    txHashes[int.Parse(item.Key)] = item.Value;
                }
                completeMethod?.Invoke(OperationStatus.Complete, null, txHashes);


            }
            //}
            //catch (Exception e)
            //{
            //    completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
            //    return;
            //}

        }


        internal void SendMultipleTransactions(TransactionToSign[] transactions, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            Debug.Log("Send multiple");
            TransactionProcessed[] processedTransactions = new TransactionProcessed[transactions.Length];
            for (int i = 0; i < transactions.Length; i++)
            {
                switch (transactions[i].type)
                {
                    case TransactionType.EGLD:
                        processedTransactions[i] = new TransactionProcessed(transactions[i]);
                        break;
                    case TransactionType.ESDT:
                        processedTransactions[i] = SetupESDTTransaction(transactions[i], completeMethod);
                        break;
                }
                if (processedTransactions[i] == null)
                {
                    return;
                }
            }
            SendTransaction(processedTransactions, completeMethod);
        }

        private TransactionProcessed SetupESDTTransaction(TransactionToSign transaction, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            OperationResult result = Utilities.IsNumberValid(ref transaction.value);
            if (result.status == OperationStatus.Error)
            {
                completeMethod?.Invoke(result.status, result.message, null);
                return null;
            }

            //TODO verificare token
            if (transaction.token == null)
            {
                completeMethod?.Invoke(result.status, "Token not valid", null);
                return null;
            }

            //https://docs.elrond.com/tokens/esdt-tokens/
            //the GasLimit must be set to the value required by the protocol for ESDT transfers, namely 500000
            long gas = 500000;
            return SetupSCMethod(transaction.destination, "ESDTTransfer", gas, completeMethod, TokenIdentifierValue.From(transaction.token.Ticker), NumericValue.TokenAmount(TokenAmount.ESDT(transaction.value, transaction.token)));
        }


        /// <summary>
        /// Sign multiple MultiversX transactions
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        //internal async Task<string> BatchSignTransaction(TransactionData transaction)
        //{
        //    //walletConnect.OpenMobileWallet();
        //    //var results = await WalletConnect.ActiveSession.ErdBatchSignTransaction(transaction);

        //    //return results;
        //}
        #endregion


        #region CheckTransactionStatus
        /// <summary>
        /// Check if a broadcasted transaction was executed and if it was successful
        /// </summary>
        /// <param name="txHash">the hash of the transaction</param>
        /// <param name="completeMethod">callback method</param>
        /// <param name="refreshTime">time interval to query the blockchain for the status of the transaction. Lower times means more calls to blockchain APIs</param>
        internal void CheckTransactionStatus(string[] txHash, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            Debug.Log(txHash);
            try
            {
                MultiversXTransaction[] txs = new MultiversXTransaction[txHash.Length];
                for(int i=0;i<txHash.Length;i++)
                {
                    txs[i] = new MultiversXTransaction(txHash[i]);
                }
               
                Sync(txs, completeMethod, refreshTime);
            }

            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}");
                return;
            }

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
        void Sync(MultiversXTransaction[] tx, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            StartCoroutine(CheckTransaction(tx, completeMethod, refreshTime));
        }


        /// <summary>
        /// Coroutine to wait before chacking the status
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="completeMethod"></param>
        /// <param name="refreshTime"></param>
        /// <returns></returns>
        private IEnumerator CheckTransaction(MultiversXTransaction[] tx, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            yield return new WaitForSeconds(refreshTime);
            SyncTransaction(tx, completeMethod, refreshTime);
        }


        /// <summary>
        /// Check if the current transaction is executed or try again if not
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="completeMethod"></param>
        /// <param name="refreshTime"></param>
        private async void SyncTransaction(MultiversXTransaction[] tx, UnityAction<OperationStatus, string> completeMethod, float refreshTime)
        {
            bool executed = true;
            for (int i = 0; i < tx.Length; i++)
            {
                await tx[i].Sync(multiversXProvider);
                if (!tx[i].IsExecuted())
                {
                    Debug.Log($"tx {i} not executed");
                    executed = false;
                }
            }
            if (executed)
            {
                for (int i = 0; i < tx.Length; i++)
                {
                    Debug.Log($"tx {i} status {tx[i].Status}");
                    string message;
                    if (!tx[i].EnsureTransactionSuccess(out message))
                    {
                        completeMethod?.Invoke(OperationStatus.Error,$"{tx[i].TxHash} {message}");
                    }
                    else
                    {
                        completeMethod?.Invoke(OperationStatus.Complete, $"{tx[i].TxHash} {tx[0].Status}");
                    }
                }
            }
            else
            {
                Sync(tx, completeMethod, refreshTime);
            }
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
        internal void SendESDTTransaction(TransactionToSign transaction, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            TransactionProcessed processedTransaction = SetupESDTTransaction(transaction, completeMethod);
            if (processedTransaction != null)
            {
                SendTransaction(new TransactionProcessed[] { processedTransaction }, completeMethod);
            }
            //    OperationResult result = Utilities.IsNumberValid(ref transaction.value);
            //    if (result.status == OperationStatus.Error)
            //    {
            //        completeMethod?.Invoke(result.status, result.message);
            //        return;
            //    }

            //    //https://docs.elrond.com/tokens/esdt-tokens/
            //    //the GasLimit must be set to the value required by the protocol for ESDT transfers, namely 500000
            //    long gas = 500000;

            //    CallSCMethod(transaction.destination, "ESDTTransfer", gas, completeMethod, TokenIdentifierValue.From(transaction.token.Ticker), NumericValue.TokenAmount(TokenAmount.ESDT(transaction.value, transaction.token)));
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
        internal void SendNFT(string destinationAddress, string collectionIdentifier, ulong nonce, int quantity, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            //https://docs.elrond.com/tokens/nft-tokens/#tab-group-43-content-44
            long gas = 1000000;

            SetupSCMethod(connectedAccount.Address.ToString(),
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
        internal TransactionProcessed SetupSCMethod(string scAddress, string methodName, long gasRequiredForSCExecution, UnityAction<OperationStatus, string, string[]> completeMethod, params IBinaryType[] args)
        {
            string data = methodName;
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCoder.EncodeTopLevel(arg))}");
            }

            return new TransactionProcessed(scAddress, 0.ToString(), data, gasRequiredForSCExecution);

            //SendTransaction(new TransactionToSign[] { new TransactionToSign(scAddress, 0.ToString(), data) }, completeMethod, gasRequiredForSCExecution);
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
            OpenDeepLink();
        }


        /// <summary>
        /// Check if wallet is connected 
        /// </summary>
        /// <returns></returns>
        internal bool IsWalletConnected()
        {
            return walletConnect.IsConnected();
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
        internal async void Disconnect()
        {
            await walletConnect.Disconnect();
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

            if (apiSettings == null || string.IsNullOrEmpty(apiSettings.selectedAPIName))
            {
                Debug.LogError("No Settings found. Go to Tools->MultiversX Tools->Settings Window and save your settings first");
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
    }
}


