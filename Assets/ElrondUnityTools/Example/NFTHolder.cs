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


        public void Initialize(DemoScript demoScript, string name, string collectionIdentifier, int nonce)
        {
            this.demoScript = demoScript;
            this.name.text = name;
            this.collectionIdentifier = collectionIdentifier;
            this.nonce = nonce;
        }


        public void SetImage(Sprite image)
        {
            this.image.sprite = image;
        }


        public void SendNFT()
        {
            ElrondUnityTools.Manager.SendNFT(demoScript.nftDestination.text, collectionIdentifier, nonce, 1, CompleteListener);
        }


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