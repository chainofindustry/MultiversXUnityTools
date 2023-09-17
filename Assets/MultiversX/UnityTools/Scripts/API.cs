using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Data.Transactions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiversX.UnityTools
{
    public class API
    {
        #region Required
        /// <summary>
        /// This is required before making any blockchain transaction. It is used to initialize the WalletConnect socket used for xPortal connection.
        /// </summary>
        /// <param name="OnWalletConnected">Callback triggered when user wallet connected</param>
        /// <param name="OnWalletDisconnected">Callback triggered when user wallet disconnected</param>
        /// <param name="qrImage">The image component that will display the QR for xPortal login</param>
        public static void Connect(UnityAction<CompleteCallback<Account>> OnWalletConnected, UnityAction OnWalletDisconnected, Image qrImage)
        {
            ConnectionManager.Instance.Connect(OnWalletConnected, OnWalletDisconnected, null, qrImage);
        }


        /// <summary>
        /// This is required before making any blockchain transaction. It is used to initialize the WalletConnect socket used for xPortal connection. 
        /// Has an intermediary event OnSessionConnected
        /// </summary>
        /// <param name="OnWalletConnected">Callback triggered when user wallet connected</param>
        /// <param name="OnWalletDisconnected">Callback triggered when user wallet disconnected</param>
        /// <param name="OnSessionConnected">When trigger the connection is established. qr code can be displayed</param>
        /// <param name="qrImage"></param>
        public static void Connect(UnityAction<CompleteCallback<Account>> OnWalletConnected, UnityAction OnWalletDisconnected, UnityAction<string> OnSessionConnected, Image qrImage)
        {
            ConnectionManager.Instance.Connect(OnWalletConnected, OnWalletDisconnected, OnSessionConnected, qrImage);
        }


        /// <summary>
        /// Simple check for connection status
        /// </summary>
        /// <returns>true - if connection to the wallet is active</returns>
        public static bool IsWalletConnected()
        {
            return ConnectionManager.Instance.IsWalletConnected();
        }


        /// <summary>
        /// Returns the current connected account
        /// </summary>
        /// <returns>Informations about current account</returns>
        public static Account GetConnectedAccount()
        {
            return ConnectionManager.Instance.GetConnectedAccount();
        }


        /// <summary>
        /// Network config is required if direct interaction with MVX SDK is needed
        /// </summary>
        /// <returns></returns>
        public static NetworkConfig GetNetworkConfig()
        {
            return ConnectionManager.Instance.GetNetworkConfig();
        }


        /// <summary>
        /// Load network config from API
        /// </summary>
        /// <param name="completeMethod">called when config was loaded, use GetNetworkConfig to actually get it</param>
        public static void LoadNetworkConfig(UnityAction<CompleteCallback<NetworkConfig>> completeMethod)
        {
            ConnectionManager.Instance.LoadNetworkConfig(completeMethod);
        }


        /// <summary>
        /// Login from the same mobile device that has the xPortal app already installed. It will automatically open the xPortal app.
        /// </summary>
        public static void DeepLinkLogin()
        {
            ConnectionManager.Instance.DeepLinkLogin();
        }


        /// <summary>
        /// Close the wallet connection
        /// </summary>
        public static void Disconnect()
        {
            ConnectionManager.Instance.Disconnect();
        }

        /// <summary>
        /// Generic method to sign any transaction created with MVX SDK
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="completeMethod"></param>
        public static void SignMultiplStrasactions(TransactionRequest[] transactions, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            ConnectionManager.Instance.SendTransactions(transactions, completeMethod);
        }


        /// <summary>
        /// Sign a message with your wallet address
        /// </summary>
        /// <param name="message"></param>
        /// <param name="completeMethod"></param>
        public static void SignMessage(string message, UnityAction<CompleteCallback<SignableMessage>> completeMethod)
        {
            ConnectionManager.Instance.SignMessage(message, completeMethod);
        }


        /// <summary>
        /// Check the status of a specific transaction 
        /// </summary>
        /// <param name="txHash">The hash of the transaction obtained after signing</param>
        /// <param name="completeMethod">Callback to track the result</param>
        /// <param name="refreshTime">Time to wait before querying the tx status. A tx takes some time to process so some delays are good to limit the usage of the APIs</param>
        public static void CheckTransactionStatus(string[] txHash, UnityAction<CompleteCallback<Transaction[]>> completeMethod, float refreshTime)
        {
            ConnectionManager.Instance.CheckTransactionsStatus(txHash, completeMethod, refreshTime);
        }


        /// <summary>
        /// Call any API method from the MultiversX Network
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="url">Get API url</param>
        /// <param name="completeMethod">Complete listener (operation status, error message, return data)</param>
        public static void GetRequest<T>(IUnityProvider provider, string url, UnityAction<CompleteCallback<T>> completeMethod)
        {
            ConnectionManager.Instance.GetRequest(provider, url, completeMethod);
        }


        /// <summary>
        /// Make a POST request to MultiversX APIs
        /// </summary>
        /// <param name="url">Post url</param>
        /// <param name="jsonData">json data to send</param>
        /// <param name="completeMethod">Complete listener (operation status, error message, return data)</param>
        public static void PostRequest<T>(IUnityProvider provider, string url, string jsonData, UnityAction<CompleteCallback<T>> completeMethod)
        {
            ConnectionManager.Instance.PostRequest(provider, url, jsonData, completeMethod);
        }

        public static IApiProviderUnity GetApiProvider()
        {
            return ConnectionManager.Instance.GetApiProvider();
        }

        public static IGatewayProviderUnity GetGatewayProvider()
        {
            return ConnectionManager.Instance.GetGatewayProvider();
        }

        /// <summary>
        /// Get the current config settings saved using the Settings Window
        /// </summary>
        /// <returns></returns>
        public static AppSettings GetApiSettings()
        {
            return ConnectionManager.Instance.GetApiSettings();
        }
        #endregion


        #region Optional
        /// <summary>
        /// Send an EGLD transaction for signing to the xPortal wallet. After the signing the transaction will be automatically broadcasted to the blockchain 
        /// </summary>
        /// <param name="destinationAddress">The erd address of the receiver</param>
        /// <param name="amount">Amount of EGLD to send(in decimals) as string</param>
        /// <param name="data">An optional custom message</param>
        /// <param name="completeMethod">Callback to track the status of the transaction. At complete, the message will be the transaction hash</param>
        public static void SendEGLDTransaction(string destinationAddress, string amount, string data, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            ConnectionManager.Instance.SendMultipleTransactions(new TransactionToSign[] { new TransactionToSign(destinationAddress, amount, data) }, completeMethod);
        }


        /// <summary>
        /// Send an ESDT transaction for signing to the xPortal wallet. After it is signed the transaction will be automatically broadcasted to the blockchain 
        /// </summary>
        /// <param name="destinationAddress">The erd address of the receiver</param>
        /// <param name="token">Token to send</param>
        /// <param name="amount">Amount of token to send(in decimals) as string</param>
        /// <param name="completeMethod">Callback to track the status of the transaction. At complete, the message will be the transaction hash</param>
        public static void SendESDTTransaction(string destinationAddress, ESDT token, string amount, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            ConnectionManager.Instance.SendMultipleTransactions(new TransactionToSign[] { new TransactionToSign(destinationAddress, token, amount) }, completeMethod);
        }


        /// <summary>
        /// Send an NFT to the destination address
        /// </summary>
        /// <param name="destinationAddress">The address to send the NFT to</param>
        /// <param name="collectionIdentifier">The collection ID</param>
        /// <param name="nftNonce">Nonce of the NFT (the characters after the last -(dash) from the NFT identifier)</param>
        /// <param name="quantity">Number of units to send</param>
        /// <param name="completeMethod">Callback to check the transaction status</param>
        public static void SendNFT(string destinationAddress, string collectionIdentifier, ulong nftNonce, int quantity, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            ConnectionManager.Instance.SendMultipleTransactions(new TransactionToSign[] { new TransactionToSign(destinationAddress, collectionIdentifier, nftNonce, quantity) }, completeMethod);
        }


        /// <summary>
        /// Call a smart contract method
        /// </summary>
        /// <param name="scAddress">The address of the smart contract</param>
        /// <param name="methodName">The method to call</param>
        /// <param name="gas">The gas required to execute the called SC method</param>
        /// <param name="completeMethod">Callback to get the result of the call</param>
        /// <param name="args">The list of arguments. Can be:</param>
        /// Address
        /// BooleanValue
        /// BytesValue
        /// EnumValue
        /// MultiValue
        /// NumericValue
        /// OptionValue
        /// StructValue
        /// TokenIdentifierValue
        public static void CallSCMethod(string scAddress, string methodName, long gas, UnityAction<CompleteCallback<string[]>> completeMethod, params IBinaryType[] args)
        {
            ConnectionManager.Instance.SendMultipleTransactions(new TransactionToSign[] { new TransactionToSign(scAddress, methodName, gas, args) }, completeMethod);
        }


        /// <summary>
        /// Make a smart contract query
        /// </summary>
        /// <typeparam name="T">Query response type</typeparam>
        /// <param name="scAddress">The address of the smart contract</param>
        /// <param name="methodName">The method to call</param>
        /// <param name="completeMethod">Callback to get the result of the query after finished</param>
        /// <param name="outputType">Type of expected result</param>
        /// <param name="args">The list of arguments</param>        
        public static void MakeSCQuery<T>(string scAddress, string methodName, UnityAction<CompleteCallback<T>> completeMethod, TypeValue[] outputType, params IBinaryType[] args) where T : IBinaryType
        {
            ConnectionManager.Instance.MakeSCQuery(scAddress, methodName, completeMethod, outputType, args);
        }


        /// <summary>
        /// Send multiple transactions to sign in a single call
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="completeMethod"></param>
        public static void SendMultipletrasactions(TransactionToSign[] transactions, UnityAction<CompleteCallback<string[]>> completeMethod)
        {
            ConnectionManager.Instance.SendMultipleTransactions(transactions, completeMethod);
        }


        /// <summary>
        /// Load metadata from all NFT properties from the connected wallet. From the metadata the media files can be downloaded  
        /// </summary>
        /// <param name="completeMethod">Callback triggered on load finish</param>
        public static void LoadWalletNFTs(UnityAction<CompleteCallback<NFTMetadata[]>> completeMethod)
        {
            ConnectionManager.Instance.LoadWalletNFTs(completeMethod);
        }


        /// <summary>
        /// Refresh the account balance and nonce
        /// </summary>
        /// <param name="completeMethod"></param>
        public static void RefreshAccount(UnityAction<CompleteCallback<Account>> completeMethod = null)
        {
            ConnectionManager.Instance.RefreshAccount(completeMethod);
        }


        /// <summary>
        /// Load all ESDT tokens from an account
        /// </summary>
        /// <param name="completeMethod">Callback triggered on load finish</param>
        public static void LoadAllTokens(UnityAction<CompleteCallback<TokenMetadata[]>> completeMethod)
        {
            ConnectionManager.Instance.LoadAllTokens(completeMethod);
        }


        /// <summary>
        /// Load an image from and URL and display it directly on an image component when loaded
        /// </summary>
        /// <param name="imageURL">url to download image from</param>
        /// <param name="displayComponent">the image component to display the picture</param>
        public static void LoadImage(string imageURL, Image displayComponent, UnityAction<CompleteCallback<Texture2D>> completeMethod)
        {
            ConnectionManager.Instance.LoadImage(imageURL, displayComponent, completeMethod);
        }
        #endregion
    }
}