using MultiversXUnityTools;
using Mx.NET.SDK.Domain.Data.Transaction;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversXUnityExamples
{
    /// <summary>
    /// This class is added to NFT Holder GameObject to display an NFT
    /// </summary>
    public class NFTHolder : MonoBehaviour
    {
        public Image image;
        public Text nftName;
        public string collectionIdentifier;
        public ulong nonce;

        private NftsScreen demoScript;
        //private string txHash;


        /// <summary>
        /// Initialize properties with values from the NFT
        /// </summary>
        /// <param name="demoScript">Main script</param>
        /// <param name="name">the display name for NFT</param>
        /// <param name="collectionIdentifier">collection identifier</param>
        /// <param name="nonce">nonce of the NFT (the characters after the last -(dash) from the NFT identifier)</param>
        public void Initialize(NftsScreen demoScript, string name, string collectionIdentifier, ulong nonce)
        {
            this.demoScript = demoScript;
            this.nftName.text = name;
            this.collectionIdentifier = collectionIdentifier;
            this.nonce = nonce;
        }


        //linked to the send button on screen
        public void SendNFT()
        {
            Manager.SendNFT(demoScript.nftDestination.text, collectionIdentifier, nonce, 1, CompleteListener);
        }


        /// <summary>
        /// Track the status of the send NFT transaction
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">if the operation status is complete, the message is the txHash</param>
        private void CompleteListener(CompleteCallback<string[]> result)
        {

            if (result.status == OperationStatus.Success)
            {
                Manager.CheckTransactionStatus(result.data, BlockchainTransactionListener, 1);
                demoScript.status.text = $"Pending Tx: {result.data[0]}";
            }
            else
            {
                //do something
                demoScript.status.text = $"Transaction status: {result.status}. Message: {result.errorMessage}";
            }
        }


        /// <summary>
        /// Listener for the transaction status response
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void BlockchainTransactionListener(CompleteCallback<Transaction[]> result)
        {
            demoScript.status.text = $"Tx: {result.data[0].TxHash} : {result.data[0].Status}";
            if (result.status == OperationStatus.Success)
            {
                demoScript.RefreshNFTs(collectionIdentifier, nonce);
                Destroy(gameObject);
            }
            if(result.status == OperationStatus.Error)
            {
                demoScript.status.text = $"Tx: {result.data[0].TxHash} : {result.data[0].Status} {result.data[0].GetLogs()}\n";
            }
        }
    }
}