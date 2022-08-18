using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class NFTHolder : MonoBehaviour
    {
        public Image image;
        public Text name;
        public string collectionIdentifier;
        public int nonce;

        private DemoScript demoScript;
        private string txHash;


        /// <summary>
        /// Initialize properties with values from the NFT
        /// </summary>
        /// <param name="demoScript">Main script</param>
        /// <param name="name">the display name for NFT</param>
        /// <param name="collectionIdentifier">collection identifier</param>
        /// <param name="nonce">nonce of the NFT (the characters after the last -(dash) from the NFT identifier)</param>
        public void Initialize(DemoScript demoScript, string name, string collectionIdentifier, int nonce)
        {
            this.demoScript = demoScript;
            this.name.text = name;
            this.collectionIdentifier = collectionIdentifier;
            this.nonce = nonce;
        }

        //linked to the send button on screen
        public void SendNFT()
        {
            ElrondUnityTools.Manager.SendNFT("erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf", "ADT-8daf0d", 3560, 1, CompleteListener);

            ElrondUnityTools.Manager.SendNFT(demoScript.nftDestination.text, collectionIdentifier, nonce, 1, CompleteListener);
        }


        /// <summary>
        /// Track the status of the send NFT transaction
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">if the operation status is complete, the message is the txHash</param>
        private void CompleteListener(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            demoScript.nftStatus.text = operationStatus + " " + message;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                txHash = message;
                Debug.Log("Tx Hash: " + txHash);
                ElrondUnityTools.Manager.CheckTransactionStatus(txHash, BlockchainTransactionListener, 1);
            }
            if (operationStatus == ElrondUnityTools.OperationStatus.Error)
            {
                //do something
            }
        }


        /// <summary>
        /// Listener for the transaction status response
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void BlockchainTransactionListener(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            demoScript.nftStatus.text = operationStatus + " " + message;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                if (message == "pending")
                {
                    ElrondUnityTools.Manager.CheckTransactionStatus(txHash, BlockchainTransactionListener, 1);
                }
                else
                {
                    if (message == "success")
                    {
                        demoScript.RefreshNFTs(collectionIdentifier, nonce);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}