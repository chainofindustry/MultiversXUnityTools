using Erdcsharp.Provider.Dtos;
using UnityEngine.Events;

namespace ElrondUnityTools
{
    public class Manager
    {
        /// <summary>
        /// This is required before making any blockchain transaction. It is used to initialize the WalletConnect socket used for Maiar connection.
        /// </summary>
        /// <param name="OnWalletConnected">Callback triggered when user wallet connected</param>
        /// <param name="OnWalletDisconnected">Callback triggered when user wallet disconnected</param>
        /// <param name="qrImage">The image component that will display the QR for Maiar login</param>
        public static void Connect(UnityAction<AccountDto> OnWalletConnected, UnityAction OnWalletDisconnected, UnityEngine.UI.Image qrImage)
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
        /// <param name="amount">Amount of EGLD to send(in decimals)</param>
        /// <param name="data">An optional custom message</param>
        /// <param name="TransactionStatus">Callback to track the status of the transaction. At complete, the message will be the transaction hash</param>
        public static void SendTransaction(string destinationAddress, string amount, string data, UnityAction<OperationStatus, string> TransactionStatus)
        {
            ConnectionManager.Instance.SendEGLDTransaction(destinationAddress, amount, data, TransactionStatus);
        }


        /// <summary>
        /// Check the status of a specific transaction 
        /// </summary>
        /// <param name="txHash">The hash of the transaction obtained after signing</param>
        /// <param name="TransactionStatus">Callback to track the result</param>
        /// <param name="delay">Time to wait before querying the tx status. A tx takes some time to process so some delays are good to limit the usage of APIs</param>
        public static void CheckTransactionStatus(string txHash, UnityAction<OperationStatus, string> TransactionStatus, float delay)
        {
            ConnectionManager.Instance.CheckTransactionStatus(txHash, TransactionStatus, delay);
        }


        /// <summary>
        /// Send an ESDT transaction for signing to the Maiar wallet. After the signing the transaction will be automatically broadcasted to the blockchain 
        /// </summary>
        /// <param name="destinationAddress">The erd address of the receiver</param>
        /// <param name="token">Token to send</param>
        /// <param name="amount">Amount of token to send(in decimals)</param>
        /// <param name="TransactionStatus">Callback to track the status of the transaction. At complete, the message will be the transaction hash</param>
        public static void SendESDTTransaction(string destinationAddress, ESDTToken token, string amount, UnityAction<OperationStatus, string> TransactionStatus)
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
        /// <param name="destinationAddress">the address to send the NFT to</param>
        /// <param name="collectionIdentifier">the collection ID</param>
        /// <param name="nftNonce">nonce of the NFT (the characters after the last -(dash) from the NFT identifier)</param>
        /// <param name="quantity">number of units to send</param>
        /// <param name="TransactionStatus">Callback to check the transaction status</param>
        public static void SendNFT(string destinationAddress, string collectionIdentifier, int nftNonce, int quantity, UnityAction<OperationStatus, string> TransactionStatus)
        {
            ConnectionManager.Instance.SendNFT(destinationAddress, collectionIdentifier, nftNonce, quantity, TransactionStatus);
        }

        public static void MakeSCQuery(string scAddress, string methodName, string[] args, UnityAction<OperationStatus, string, SCData> QueryComplete)
        {
            ConnectionManager.Instance.MakeSCQuery(scAddress, methodName, args, QueryComplete);
        }
    }
}