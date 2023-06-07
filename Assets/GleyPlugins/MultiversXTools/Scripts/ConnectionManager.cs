using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Codec;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Account;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Data.Transaction;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Domain.SmartContracts;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.TransactionsManager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

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
                networkConfig = await LoadNetworkConfig(true, true);

                if (walletConnect == null)
                {
                    walletConnect = gameObject.AddComponent<WalletConnect>();
                }

                WalletConnectSharp.Core.Models.Pairing.Metadata metadata = new WalletConnectSharp.Core.Models.Pairing.Metadata()
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

                connectedAccount = new Account(Address.From(account));
                OnWalletConnected?.Invoke(connectedAccount, null);
            }
            catch (Exception e)
            {
                OnWalletConnected?.Invoke(null, $"{e.Message}");
            }
        }


        /// <summary>
        /// xPortal deep link urls
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
                Debug.LogError($"{e.Message}");
                return;
            }

            if (selectedAPI == null)
            {
                Debug.LogError("Null API selected");
                return;
            }

            Debug.Log($"SELECTED API {selectedAPI.apiName} {selectedAPI.baseAddress}");
            multiversXProvider = new MultiversXProviderUnity(selectedAPI);
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
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}");
            }
        }
        #endregion


        internal async void SignMessage(string message, UnityAction<OperationStatus, string> completeMethod)
        {
            Debug.Log("Sign message");
            try
            {
                completeMethod?.Invoke(OperationStatus.InProgress, "Waiting to sign");
                string signature = await walletConnect.SignMessage(message);
                Debug.Log(signature);
                completeMethod?.Invoke(OperationStatus.Complete, signature);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}");
            }
        }

        #region Transactions
        /// <summary>
        /// Actually constructs the transaction, send it for signing, and broadcast the signed transaction to the blockchain
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="amount"></param>
        /// <param name="data"></param>
        /// <param name="completeMethod"></param>
        /// <param name="completeMethod"></param>
        /// <param name="requiredGas"></param>
        internal async void SendTransactions(TransactionRequest[] transactionsToSign, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            //wait for the signature from wallet
            try
            {
                completeMethod?.Invoke(OperationStatus.InProgress, "Waiting to sign", null);
                TransactionRequestDto[] signedTransactions = await walletConnect.MultiSign(transactionsToSign);
                MultipleTransactionsResponseDto response = await multiversXProvider.SendTransactions(signedTransactions);

                string[] txHashes = new string[response.NumOfSentTxs];
                foreach (var item in response.TxsHashes)
                {
                    txHashes[int.Parse(item.Key)] = item.Value;
                }
                completeMethod?.Invoke(OperationStatus.Complete, null, txHashes);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}", null);
                return;
            }

        }


        internal async void SendMultipleTransactions(TransactionToSign[] transactions, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            TransactionRequest[] processedTransactions = new TransactionRequest[transactions.Length];
            for (int i = 0; i < transactions.Length; i++)
            {
                //verify the address
                if (!Utilities.IsAddressValid(transactions[i].destination))
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
                    completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}", null);
                    return;
                }

                try
                {
                    await connectedAccount.Sync(multiversXProvider);
                }
                catch (Exception e)
                {
                    completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}", null);
                    return;
                }


                switch (transactions[i].type)
                {
                    case TransactionType.EGLD:
                        processedTransactions[i] = SetupEgldTransaction(transactions[i], completeMethod);
                        break;
                    case TransactionType.ESDT:
                        processedTransactions[i] = SetupESDTTransaction(transactions[i], completeMethod);
                        break;
                    case TransactionType.NFT:
                        processedTransactions[i] = SetupNFTTransaction(transactions[i], completeMethod);
                        break;
                    case TransactionType.SC:
                        processedTransactions[i] = SetupSCMethod(transactions[i].destination, transactions[i].methodName, transactions[i].gas, transactions[i].args);
                        break;
                }
                if (processedTransactions[i] == null)
                {
                    return;
                }
            }
            SendTransactions(processedTransactions, completeMethod);
        }

        private TransactionRequest SetupEgldTransaction(TransactionToSign transaction, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            //check EGLD balance
            if (ESDTAmount.EGLD(transaction.value).Value > connectedAccount.Balance.Value)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"Insufficient funds, required : {ESDTAmount.EGLD(transaction.value).ToDenominated()} and got {connectedAccount.Balance.ToDenominated()}", null);
                return null;
            }

            //make sure the amount value is correct
            OperationResult result = Utilities.IsNumberValid(ref transaction.value);
            if (result.status == OperationStatus.Error)
            {
                completeMethod?.Invoke(result.status, result.message, null);
                return null;
            }

            return TransactionRequest.CreateEgldTransactionRequest(networkConfig, connectedAccount, Address.FromBech32(transaction.destination), ESDTAmount.EGLD(transaction.value), transaction.data);
        }


        private TransactionRequest SetupESDTTransaction(TransactionToSign transaction, UnityAction<OperationStatus, string, string[]> completeMethod)
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

            return TokenTransactionRequest.TokenTransfer(networkConfig, connectedAccount, Address.FromBech32(transaction.destination), ESDTIdentifierValue.From(transaction.token.Identifier), ESDTAmount.ESDT(transaction.value, transaction.token));
        }


        private TransactionRequest SetupNFTTransaction(TransactionToSign transaction, UnityAction<OperationStatus, string, string[]> completeMethod)
        {
            return ESDTTransactionRequest.NFTTransfer(
                networkConfig,
                connectedAccount,
                Address.FromBech32(transaction.destination),
                ESDTIdentifierValue.From(transaction.collectionIdentifier),
                transaction.nftNonce,
                ESDTAmount.From(transaction.quantity));
        }


        /// <summary>
        /// Perform a Smart Contract call 
        /// </summary>
        /// <param name="scAddress"></param>
        /// <param name="methodName"></param>
        /// <param name="gasRequiredForSCExecution"></param>
        /// <param name="completeMethod"></param>
        /// <param name="args"></param>
        internal TransactionRequest SetupSCMethod(string scAddress, string methodName, long gasRequiredForSCExecution, params IBinaryType[] args)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig, connectedAccount, Address.FromBech32(scAddress), ESDTAmount.Zero(), methodName, args);
            transaction.SetGasLimit(new GasLimit(gasRequiredForSCExecution));
            return transaction;
        }
        #endregion


        #region CheckTransactionStatus
        /// <summary>
        /// Check if a broadcasted transaction was executed and if it was successful
        /// </summary>
        /// <param name="txHash">the hash of the transaction</param>
        /// <param name="completeMethod">callback method</param>
        /// <param name="refreshTime">time interval to query the blockchain for the status of the transaction. Lower times means more calls to blockchain APIs</param>
        internal void CheckTransactionStatus(string[] txHash, UnityAction<OperationStatus, string, string> completeMethod, float refreshTime)
        {
            try
            {
                Transaction[] txs = new Transaction[txHash.Length];
                for (int i = 0; i < txHash.Length; i++)
                {
                    txs[i] = new Transaction(txHash[i]);
                }

                Sync(txs, completeMethod, refreshTime);
            }

            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, "", e.Message);
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
        void Sync(Transaction[] tx, UnityAction<OperationStatus, string, string> completeMethod, float refreshTime)
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
        private IEnumerator CheckTransaction(Transaction[] tx, UnityAction<OperationStatus, string, string> completeMethod, float refreshTime)
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
        private async void SyncTransaction(Transaction[] tx, UnityAction<OperationStatus, string, string> completeMethod, float refreshTime)
        {
            bool executed = true;
            for (int i = 0; i < tx.Length; i++)
            {
                try
                {
                    await tx[i].Sync(multiversXProvider);
                }
                catch (Exception e)
                {
                    completeMethod?.Invoke(OperationStatus.Error, tx[i].TxHash, e.Message);
                    return;
                }
                if (!tx[i].IsExecuted())
                {
                    executed = false;
                }
            }
            if (executed)
            {
                for (int i = 0; i < tx.Length; i++)
                {
                    try
                    {
                        tx[i].EnsureTransactionSuccess();
                        completeMethod?.Invoke(OperationStatus.Complete, tx[i].TxHash, tx[i].Status);
                    }
                    catch
                    {
                        string logs = tx[i].Status;

                        if (tx[i].Logs != null)
                        {
                            if (tx[i].Logs.Events != null)
                            {
                                for (int j = 0; j < tx[i].Logs.Events.Length; j++)
                                {
                                    logs += " " + tx[i].Logs.Events[j].Identifier;
                                    if (tx[i].Logs.Events[j].Topics != null)
                                    {
                                        if (tx[i].Logs.Events[j].Topics.Length > 0)
                                        {
                                            for (int k = 1; k < tx[i].Logs.Events[j].Topics.Length; k++)
                                            {
                                                logs += $" {Encoding.UTF8.GetString(Convert.FromBase64String(tx[i].Logs.Events[j].Topics[k]))}";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Debug.LogWarning(logs);
                        completeMethod?.Invoke(OperationStatus.Error, tx[i].TxHash, logs);
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
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}", null);
                return;
            }
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
            try
            {
                var queryResult = await SmartContract.QuerySmartContract<T>(multiversXProvider, Address.From(scAddress), outputType, methodName, connectedAccount.Address, args);
                completeMethod(OperationStatus.Complete, "Success", queryResult);
            }
            catch (APIException e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Error}: {e.Message}", default(T));
            }
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
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}", default(T));
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
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}", default(T));
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
        /// Automatically open the xPortal wallet on mobile devices
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

        internal NetworkConfig GetNetworkConfig()
        {
            return networkConfig;
        }

        internal async void LoadNetworkConfig(UnityAction<OperationStatus, string> completeMethod)
        {
            try
            {
                networkConfig = await LoadNetworkConfig(true);
                completeMethod?.Invoke(OperationStatus.Complete, $"Success");
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Message}");
                return;
            }
        }
        #endregion
    }
}


