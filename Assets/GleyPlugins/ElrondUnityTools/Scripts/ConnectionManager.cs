using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.SmartContracts;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private IElrondApiProvider elrondAPI;
        private NetworkConfig networkConfig;
        private UnityAction<Account> OnWalletConnected;
        private UnityAction OnWalletDisconnected;
        private WalletConnect walletConnect;
        private bool walletConnected;
        private BinaryCodec BinaryCoder = new BinaryCodec();

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

            APISettings apiSettings = Resources.Load<APISettings>("APISettingsData");
            if (apiSettings == null || string.IsNullOrEmpty(apiSettings.selectedAPIName))
            {
                Debug.LogError("No API settings file found -> Go to ... and generate one");
                return;
            }

            API selectedAPI;
            StreamReader reader = new StreamReader($"{Application.dataPath}/GleyPlugins/ElrondUnityTools/Scripts/Provider/APIs/{apiSettings.selectedAPIName}.json");
            try
            {
                Debug.Log($"{Application.dataPath}/GleyPlugins/ElrondUnityTools/Scripts/Provider/APIs/{apiSettings.selectedAPIName}.json");
                selectedAPI = JsonConvert.DeserializeObject<API>(reader.ReadToEnd());
                reader.Close();
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

           

            elrondAPI = new ElrondProviderUnity(selectedAPI);
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

            requiredGas += networkConfig.MinGasLimit + System.Text.Encoding.ASCII.GetBytes(data).Length * networkConfig.GasPerDataByte;

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
        internal async void LoadAllTokens(UnityAction<OperationStatus, string, TokenMetadata[]> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(OperationStatus.Complete, "Success", await elrondAPI.GetWalletTokens<TokenMetadata[]>(connectedAccount.Address.ToString()));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, e.Message + ": " + e.Data, null);
            }
        }
        #endregion

        #region NFTs
        public async void LoadWalletNFTs(UnityAction<OperationStatus, string, NFTMetadata[]> completeMethod)
        {
            try
            {
                List<NFTMetadata> allNfts = await elrondAPI.GetWalletNfts<List<NFTMetadata>>(connectedAccount.Address.ToString());
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
        internal async void MakeSCQuery<T>(string scAddress, string methodName, UnityAction<OperationStatus, string, T> completeMethod, TypeValue outputType, params IBinaryType[] args) where T : IBinaryType
        {
            try
            {
                var queryResult = await SmartContract.QuerySmartContract<T>(elrondAPI, Erdcsharp.Domain.Address.From(scAddress), outputType, methodName, connectedAccount.Address, args);
                completeMethod(OperationStatus.Complete, "Success", queryResult);
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", default(T));
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
        internal async void GetRequest<T>(string url, UnityAction<OperationStatus, string, T> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(OperationStatus.Complete, "Success", await elrondAPI.GetRequest<T>(url));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", default(T));
            }
        }


        internal async void PostRequest<T>(string url, string jsonData, UnityAction<OperationStatus, string, T> completeMethod)
        {
            try
            {
                completeMethod?.Invoke(OperationStatus.Complete, "Success", await elrondAPI.PostRequest<T>(url, jsonData));
            }
            catch (Exception e)
            {
                completeMethod?.Invoke(OperationStatus.Error, $"{e.Data} {e.Message}", default(T));
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


