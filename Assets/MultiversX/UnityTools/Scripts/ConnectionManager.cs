using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Data.Transactions;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Domain.SmartContracts;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using Mx.NET.SDK.TransactionsManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MultiversX.UnityTools
{
    public class ConnectionManager : MonoBehaviour
    {
        private Account connectedAccount;
        private NetworkConfig networkConfig;
        private AppSettings apiSettings;
        private WalletConnectUnity walletConnectUnity;
        private IApiProviderUnity apiProviderUnity;
        private IGatewayProviderUnity gatewayProviderUnity;


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
        internal async void Connect(UnityAction<CompleteCallback<Account>> OnWalletConnected, UnityAction OnWalletDisconnected, UnityAction<string> OnSessionConnected, Image qrImage)
        {
            apiSettings = GetApiSettings();
            if (apiSettings == null)
            {
                Debug.LogError("No API settings file found -> Go to ... and generate one");
                return;
            }

            try
            {
                apiProviderUnity = new ApiProviderUnity(new ApiNetworkConfiguration(apiSettings.selectedNetwork));
                gatewayProviderUnity = new GatewayProviderUnity(new GatewayNetworkConfiguration(apiSettings.selectedNetwork));
            }
            catch
            {
                OnWalletConnected?.Invoke(new CompleteCallback<Account>(OperationStatus.Error, $"Network not found: {apiSettings.selectedNetwork}", null));
                return;
            }
            try
            {
                networkConfig = await LoadNetworkConfig(true, true);

                WalletConnectSharp.Core.Models.Pairing.Metadata metadata = new WalletConnectSharp.Core.Models.Pairing.Metadata()
                {
                    Description = apiSettings.appDescription,
                    Icons = new[] { apiSettings.appIcon },
                    Name = apiSettings.appName,
                    Url = apiSettings.appWebsite
                };


                if (walletConnectUnity == null)
                {
                    walletConnectUnity = await gameObject.AddComponent<WalletConnectUnity>().Initialize(metadata, apiSettings.projectID, networkConfig.ChainId, Application.persistentDataPath + apiSettings.savePath);
                }

                string account = await walletConnectUnity.Connect(
                    (uri) =>
                    {
                        OnSessionConnected?.Invoke(uri);
                        AddQRImageScript(qrImage, uri);
                    },
                    OnWalletDisconnected
                    );

                connectedAccount = new Account(Address.From(account));
                OnWalletConnected?.Invoke(new CompleteCallback<Account>(OperationStatus.Success, "", connectedAccount));
            }
            catch (Exception e)
            {
                OnWalletConnected?.Invoke(new CompleteCallback<Account>(OperationStatus.Error, $"{e.Message}", null));
            }
        }


        /// <summary>
        /// xPortal deep link urls
        /// </summary>
        internal void OpenDeepLink()
        {
            walletConnectUnity.OpenDeepLink();
        }


        private void AddQRImageScript(Image qrImage, string uri)
        {
            if (qrImage != null)
            {
                qrImage.gameObject.AddComponent<WalletConnectQRImage>().Init(uri);
            }
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
                networkConfig = await NetworkConfig.GetFromNetwork(gatewayProviderUnity);
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
        internal async void RefreshAccount(UnityAction<CompleteCallback<Account>> completeMethod)
        {
            try
            {
                await connectedAccount.Sync(apiProviderUnity);
                completeMethod?.Invoke(new CompleteCallback<Account>(OperationStatus.Success, "", connectedAccount));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<Account>(OperationStatus.Error, $"{e.Message}", null));
            }
        }
        #endregion


        internal async void SignMessage(string message, UnityAction<CompleteCallback<SignableMessage>> completeMethod)
        {
            Debug.Log("Sign message");
            try
            {
                completeMethod?.Invoke(new CompleteCallback<SignableMessage>(OperationStatus.InProgress, "Waiting to sign", null));
                SignableMessage signature = await walletConnectUnity.SignMessage(message);
                completeMethod?.Invoke(new CompleteCallback<SignableMessage>(OperationStatus.Success, "", signature));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<SignableMessage>(OperationStatus.Error, $"{e.Message}", null));
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
        internal async void SendTransactions(TransactionRequest[] transactionsToSign, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            //wait for the signature from wallet
            //try
            //{
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.InProgress, "Waiting to sign", null));
                TransactionRequestDto[] signedTransactions = await walletConnectUnity.MultiSign(transactionsToSign);
            Debug.LogWarning("BEFORE");
                MultipleTransactionsResponseDto response = await apiProviderUnity.SendTransactions(signedTransactions);
            Debug.LogWarning("AFTER");
                string[] txHashes = new string[response.NumOfSentTxs];
                foreach (var item in response.TxsHashes)
                {
                    txHashes[int.Parse(item.Key)] = item.Value;
                }
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Success, "", txHashes));
            //}
            //catch (Exception e)
            //{
            //    completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, $"{e.Message}", null));
            //    return;
            //}
        }


        internal async void SendMultipleTransactions(TransactionToSign[] transactions, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            //check network config params
            NetworkConfig networkConfig = null;
            try
            {
                networkConfig = await LoadNetworkConfig(true);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, $"{e.Message}", null));
                return;
            }

            try
            {
                await connectedAccount.Sync(apiProviderUnity);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, $"{e.Message}", null));
                return;
            }


            TransactionRequest[] processedTransactions = new TransactionRequest[transactions.Length];
            for (int i = 0; i < transactions.Length; i++)
            {
                //verify the address
                if (!Utilities.IsAddressValid(transactions[i].destination))
                {
                    completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, "Invalid destination address", null));
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
                connectedAccount.IncrementNonce();
            }
            SendTransactions(processedTransactions, completeMethod);
        }

        private TransactionRequest SetupEgldTransaction(TransactionToSign transaction, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            //check EGLD balance
            if (ESDTAmount.EGLD(transaction.value).Value > connectedAccount.Balance.Value)
            {
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, $"Insufficient funds, required : {ESDTAmount.EGLD(transaction.value).ToDenominated()} and got {connectedAccount.Balance.ToDenominated()}", null));
                return null;
            }

            //make sure the amount value is correct
            OperationResult result = Utilities.IsNumberValid(ref transaction.value);
            if (result.status == OperationStatus.Error)
            {
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, result.message, null));
                return null;
            }

            return TransactionRequest.CreateEgldTransactionRequest(networkConfig, connectedAccount, Address.FromBech32(transaction.destination), ESDTAmount.EGLD(transaction.value), transaction.data);
        }


        private TransactionRequest SetupESDTTransaction(TransactionToSign transaction, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            OperationResult result = Utilities.IsNumberValid(ref transaction.value);
            if (result.status == OperationStatus.Error)
            {
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, result.message, null));
                return null;
            }

            //TODO verificare token
            if (transaction.token == null)
            {
                completeMethod?.Invoke(new CompleteCallback<string[]>(OperationStatus.Error, "Token not valid", null));
                return null;
            }

            return TokenTransactionRequest.TokenTransfer(networkConfig, connectedAccount, Address.FromBech32(transaction.destination), ESDTIdentifierValue.From(transaction.token.Identifier), ESDTAmount.ESDT(transaction.value, transaction.token));
        }


        private TransactionRequest SetupNFTTransaction(TransactionToSign transaction, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            //TODO do some checks on params
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
        private TransactionRequest SetupSCMethod(string scAddress, string methodName, long gasRequiredForSCExecution, params IBinaryType[] args)
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
        internal void CheckTransactionsStatus(string[] txHash, UnityAction<CompleteCallback<Transaction[]>> completeMethod, float refreshTime)
        {
            Sync(Transaction.From(txHash), completeMethod, refreshTime);
        }


        /// <summary>
        /// Start to sync a transaction
        /// </summary>
        /// <param name="tx">transaction</param>
        /// <param name="completeMethod">callback when completed</param>
        /// <param name="refreshTime"></param>
        private void Sync(Transaction[] txs, UnityAction<CompleteCallback<Transaction[]>> completeMethod, float refreshTime)
        {
            completeMethod?.Invoke(new CompleteCallback<Transaction[]>(OperationStatus.InProgress, "", txs));
            StartCoroutine(Wait(txs, completeMethod, refreshTime));
        }


        /// <summary>
        /// Coroutine to wait before chacking the status
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="completeMethod"></param>
        /// <param name="refreshTime"></param>
        /// <returns></returns>
        private IEnumerator Wait(Transaction[] tx, UnityAction<CompleteCallback<Transaction[]>> completeMethod, float refreshTime)
        {
            yield return new WaitForSeconds(refreshTime);
            CheckIfProcessed(tx, completeMethod, refreshTime);
        }


        /// <summary>
        /// Check if the current transaction is executed or try again if not
        /// </summary>
        /// <param name="txs"></param>
        /// <param name="completeMethod"></param>
        /// <param name="refreshTime"></param>
        private async void CheckIfProcessed(Transaction[] txs, UnityAction<CompleteCallback<Transaction[]>> completeMethod, float refreshTime)
        {
            bool executed = true;
            for (int i = 0; i < txs.Length; i++)
            {
                try
                {
                    await txs[i].Sync(apiProviderUnity);
                }
                catch (Exception e)
                {
                    completeMethod?.Invoke(new CompleteCallback<Transaction[]>(OperationStatus.Error, $"{txs[i].TxHash} : {e.Message}", txs));
                    return;
                }
                if (!txs[i].IsExecuted())
                {
                    executed = false;
                }
            }
            if (executed)
            {
                for (int i = 0; i < txs.Length; i++)
                {
                    try
                    {
                        txs[i].EnsureTransactionSuccess();
                    }
                    catch (Exception e)
                    {
                        completeMethod?.Invoke(new CompleteCallback<Transaction[]>(OperationStatus.Error, e.Message, txs));
                        return;
                    }
                }
                completeMethod?.Invoke(new CompleteCallback<Transaction[]>(OperationStatus.Success, "", txs));
            }
            else
            {
                Sync(txs, completeMethod, refreshTime);
            }
        }
        #endregion


        #region Tokens
        /// <summary>
        /// Load all tokens from a wallet
        /// </summary>
        /// <param name="completeMethod"></param>
        internal async void LoadAllTokens(UnityAction<CompleteCallback<TokenMetadata[]>> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(new CompleteCallback<TokenMetadata[]>(OperationStatus.Success, "", await apiProviderUnity.GetWalletTokens<TokenMetadata[]>(connectedAccount.Address.ToString())));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<TokenMetadata[]>(OperationStatus.Error, e.Message + ": " + e.Data, null));
            }
        }
        #endregion


        #region NFTs
        /// <summary>
        /// Load all NFTs from a wallet
        /// </summary>
        /// <param name="completeMethod"></param>
        public async void LoadWalletNFTs(UnityAction<CompleteCallback<NFTMetadata[]>> completeMethod)
        {


            try
            {
                List<NFTMetadata> allNfts = await apiProviderUnity.GetWalletNfts<List<NFTMetadata>>(connectedAccount.Address.ToString());
                for (int i = allNfts.Count - 1; i >= 0; i--)
                {
                    var medatada = allNfts[i].metadata;
                    //remove the LP tokens
                    if (allNfts[i].type == "MetaESDT")
                    {
                        allNfts.RemoveAt(i);
                    }
                }

                completeMethod?.Invoke(new CompleteCallback<NFTMetadata[]>(OperationStatus.Success, "", allNfts.ToArray()));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<NFTMetadata[]>(OperationStatus.Error, $"{e.Message}", null));
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
        internal async void MakeSCQuery<T>(string scAddress, string methodName, UnityAction<CompleteCallback<T>> completeMethod, TypeValue[] outputType, params IBinaryType[] args) where T : IBinaryType
        {
            try
            {
                var queryResult = await SmartContract.QuerySmartContract<T>(gatewayProviderUnity, Address.From(scAddress), outputType, methodName, connectedAccount.Address, args);
                completeMethod?.Invoke(new CompleteCallback<T>(OperationStatus.Success, "", queryResult));
            }
            catch (APIException e)
            {
                completeMethod?.Invoke(new CompleteCallback<T>(OperationStatus.Error, $"{e.Error}: {e.Message}", default(T)));
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
        internal async void GetRequest<T>(IUnityProvider provider, string url, UnityAction<CompleteCallback<T>> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(new CompleteCallback<T>(OperationStatus.Success, "", await provider.Get<T>(url)));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<T>(OperationStatus.Error, $"{e.Message}", default(T)));
            }
        }


        /// <summary>
        /// Make any kind of POST request using the API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="jsonData"></param>
        /// <param name="completeMethod"></param>
        internal async void PostRequest<T>(IUnityProvider provider, string url, string jsonData, UnityAction<CompleteCallback<T>> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(new CompleteCallback<T>(OperationStatus.Success, "", await provider.Post<T>(url, jsonData)));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<T>(OperationStatus.Error, $"{e.Message}", default(T)));
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
        internal void LoadImage(string imageURL, Image displayComponent, UnityAction<CompleteCallback<Texture2D>> completeMethod)
        {
            StartCoroutine(LoadImageCoroutine(imageURL, displayComponent, completeMethod));
        }

        /// <summary>
        /// Load an image from the url
        /// </summary>
        /// <param name="imageURL"></param>
        /// <param name="displayComponent">image component to display the downloaded thumbnail picture</param>
        /// <returns></returns>
        private IEnumerator LoadImageCoroutine(string imageURL, Image displayComponent, UnityAction<CompleteCallback<Texture2D>> completeMethod)
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
                        completeMethod?.Invoke(new CompleteCallback<Texture2D>(OperationStatus.Success, "", imageTex));
                    }
                    break;
                default:
                    completeMethod?.Invoke(new CompleteCallback<Texture2D>(OperationStatus.Error, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}", null));
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
            return walletConnectUnity.IsConnected();
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
            await walletConnectUnity.Disconnect();
        }


        /// <summary>
        /// Returns the current selected MultiversX API
        /// </summary>
        /// <returns></returns>
        internal AppSettings GetApiSettings()
        {
            if (apiSettings == null)
            {
                apiSettings = Resources.Load<AppSettings>(Constants.APP_SETTINGS_DATA);
            }

            if (apiSettings == null)
            {
                Debug.LogError("No Settings found. Go to Tools->MultiversX Tools->Settings Window and save your settings first");
            }

            return apiSettings;
        }

        internal NetworkConfig GetNetworkConfig()
        {
            return networkConfig;
        }

        internal async void LoadNetworkConfig(UnityAction<CompleteCallback<NetworkConfig>> completeMethod)
        {
            try
            {
                networkConfig = await LoadNetworkConfig(true);
                completeMethod?.Invoke(new CompleteCallback<NetworkConfig>(OperationStatus.Success, "", networkConfig));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(new CompleteCallback<NetworkConfig>(OperationStatus.Error, $"{e.Message}", null));
                return;
            }
        }

        internal IApiProviderUnity GetApiProvider()
        {
            return apiProviderUnity;
        }

        internal IGatewayProviderUnity GetGatewayProvider()
        {
            return gatewayProviderUnity;
        }
        #endregion
    }
}