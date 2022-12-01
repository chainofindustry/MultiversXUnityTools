using Erdcsharp.Domain;
using Erdcsharp.Domain.Values;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiversXUnityTools
{
    public class Manager
    {
        /// <summary>
        /// This is required before making any blockchain transaction. It is used to initialize the WalletConnect socket used for Maiar connection.
        /// </summary>
        /// <param name="OnWalletConnected">Callback triggered when user wallet connected</param>
        /// <param name="OnWalletDisconnected">Callback triggered when user wallet disconnected</param>
        /// <param name="qrImage">The image component that will display the QR for Maiar login</param>
        public static void Connect(UnityAction<Account> OnWalletConnected, UnityAction OnWalletDisconnected, Image qrImage)
        {
            ConnectionManager.Instance.Connect(OnWalletConnected, OnWalletDisconnected, qrImage);
        }


        /// <summary>
        /// Simple check for connection status
        /// </summary>
        /// <returns>true - if connection to the wallet is active</returns>
        public static bool IsWalletConnected()
        {
            return ConnectionManager.Instance.IsWalletConnected();
        }


        public static Account GetConnectedAccount()
        {
            return ConnectionManager.Instance.GetConnectedAccount();
        }

        /// <summary>
        /// Login from the same mobile device that has the Maiar app already installed. It will automatically open the Maiar app.
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
        /// Send an EGLD transaction for signing to the Maiar wallet. After the signing the transaction will be automatically broadcasted to the blockchain 
        /// </summary>
        /// <param name="destinationAddress">The erd address of the receiver</param>
        /// <param name="amount">Amount of EGLD to send(in decimals) as string</param>
        /// <param name="data">An optional custom message</param>
        /// <param name="TransactionStatus">Callback to track the status of the transaction. At complete, the message will be the transaction hash</param>
        public static void SendEGLDTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> TransactionStatus)
        {
            ConnectionManager.Instance.SendEGLDTransaction(destinationAddress, amount, data, TransactionStatus);
        }


        /// <summary>
        /// Check the status of a specific transaction 
        /// </summary>
        /// <param name="txHash">The hash of the transaction obtained after signing</param>
        /// <param name="TransactionStatus">Callback to track the result</param>
        /// <param name="refreshTime">Time to wait before querying the tx status. A tx takes some time to process so some delays are good to limit the usage of the APIs</param>
        public static void CheckTransactionStatus(string txHash, UnityAction<OperationStatus, string> TransactionStatus, float refreshTime)
        {
            ConnectionManager.Instance.CheckTransactionStatus(txHash, TransactionStatus, refreshTime);
        }


        /// <summary>
        /// Send an ESDT transaction for signing to the Maiar wallet. After it is signed the transaction will be automatically broadcasted to the blockchain 
        /// </summary>
        /// <param name="destinationAddress">The erd address of the receiver</param>
        /// <param name="token">Token to send</param>
        /// <param name="amount">Amount of token to send(in decimals) as string</param>
        /// <param name="TransactionStatus">Callback to track the status of the transaction. At complete, the message will be the transaction hash</param>
        public static void SendESDTTransaction(string destinationAddress, Token token, string amount, UnityAction<OperationStatus, string> TransactionStatus)
        {
            ConnectionManager.Instance.SendESDTTransaction(destinationAddress, amount, token, TransactionStatus);
        }


        /// <summary>
        /// Load metadata from all NFT properties from the connected wallet. From the metadata the media files can be downloaded  
        /// </summary>
        /// <param name="LoadNFTsComplete">Callback triggered on load finish</param>
        public static void LoadWalletNFTs(UnityAction<OperationStatus, string, NFTMetadata[]> LoadNFTsComplete)
        {
            ConnectionManager.Instance.LoadWalletNFTs(LoadNFTsComplete);
        }


        /// <summary>
        /// Send an NFT to the destination address
        /// </summary>
        /// <param name="destinationAddress">The address to send the NFT to</param>
        /// <param name="collectionIdentifier">The collection ID</param>
        /// <param name="nftNonce">Nonce of the NFT (the characters after the last -(dash) from the NFT identifier)</param>
        /// <param name="quantity">Number of units to send</param>
        /// <param name="TransactionStatus">Callback to check the transaction status</param>
        public static void SendNFT(string destinationAddress, string collectionIdentifier, ulong nftNonce, int quantity, UnityAction<OperationStatus, string> TransactionStatus)
        {
            ConnectionManager.Instance.SendNFT(destinationAddress, collectionIdentifier, nftNonce, quantity, TransactionStatus);
        }


        /// <summary>
        /// Make a smart contract query
        /// </summary>
        /// <typeparam name="T">Query response type</typeparam>
        /// <param name="scAddress">The address of the smart contract</param>
        /// <param name="methodName">The method to call</param>
        /// <param name="QueryComplete">Callback to get the result of the query after finished</param>
        /// <param name="outputType">Type of expected result</param>
        /// <param name="args">The list of arguments</param>        
        public static void MakeSCQuery<T>(string scAddress, string methodName, UnityAction<OperationStatus, string, T> QueryComplete, TypeValue outputType, params IBinaryType[] args) where T : IBinaryType
        {
            ConnectionManager.Instance.MakeSCQuery(scAddress, methodName, QueryComplete, outputType, args);
        }


        /// <summary>
        /// Call a smart contract method
        /// </summary>
        /// <param name="scAddress">The address of the smart contract</param>
        /// <param name="methodName">The method to call</param>
        /// <param name="gas">The gas required to execute the called SC method</param>
        /// <param name="CallStatus">Callback to get the result of the call</param>
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
        public static void CallSCMethod(string scAddress, string methodName, long gas, UnityAction<OperationStatus, string> CallStatus, params IBinaryType[] args)
        {
            ConnectionManager.Instance.CallSCMethod(scAddress, methodName, gas, CallStatus, args);
        }



        /// <summary>
        /// Call any API method from the MultiversX Network
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="url">Get API url</param>
        /// <param name="CompleteMethod">Complete listener (operation status, error message, return data)</param>
        public static void GetRequest<T>(string url, UnityAction<OperationStatus, string, T> CompleteMethod)
        {
            ConnectionManager.Instance.GetRequest(url, CompleteMethod);
        }


        /// <summary>
        /// Make a POST request to MultiversX APIs
        /// </summary>
        /// <param name="url">Post url</param>
        /// <param name="jsonData">json data to send</param>
        /// <param name="CompleteMethod">Complete listener (operation status, error message, return data)</param>
        public static void PostRequest<T>(string url, string jsonData, UnityAction<OperationStatus, string, T> CompleteMethod)
        {
            ConnectionManager.Instance.PostRequest(url, jsonData, CompleteMethod);
        }


        /// <summary>
        /// Refresh the account balance and nonce
        /// </summary>
        /// <param name="CompleteMethod"></param>
        public static void RefreshAccount(UnityAction<OperationStatus, string> CompleteMethod = null)
        {
            ConnectionManager.Instance.RefreshAccount(CompleteMethod);
        }


        /// <summary>
        /// Load all ESDT tokens from an account
        /// </summary>
        /// <param name="LoadTokensComplete">Callback triggered on load finish</param>
        public static void LoadAllTokens(UnityAction<OperationStatus, string, TokenMetadata[]> LoadTokensComplete)
        {
            ConnectionManager.Instance.LoadAllTokens(LoadTokensComplete);
        }


        /// <summary>
        /// Load an image from and URL and display it directly on an image component when loaded
        /// </summary>
        /// <param name="imageURL">url to download image from</param>
        /// <param name="displayComponent">the image component to display the picture</param>
        public static void LoadImage(string imageURL, Image displayComponent, UnityAction<OperationStatus, string> CompleteMethod)
        {
            ConnectionManager.Instance.LoadImage(imageURL, displayComponent, CompleteMethod);
        }

        
        /// <summary>
        /// Get the current config settings saved using the Settings Window
        /// </summary>
        /// <returns></returns>
        public static APISettings GetApiSettings()
        {
            return ConnectionManager.Instance.GetApiSettings();
        }


        /// <summary>
        /// Returns the URL associated with Endpoint
        /// </summary>
        /// <param name="endpoint">endpoint defined inside settings window</param>
        /// <returns></returns>
        public static string GetEndpointUrl(EndpointNames endpoint)
        {
            return ConnectionManager.Instance.GetEndpointUrl(endpoint);
        }

        //query vm
        //TODO
    }
}