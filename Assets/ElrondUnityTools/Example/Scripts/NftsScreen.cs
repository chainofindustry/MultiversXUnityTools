using ElrondUnityTools;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class NftsScreen : GenericUIScreen
    {
        public InputField nftDestination;
        public Text status;
        public Transform nftsHolder;
        public GameObject nftItem;

        private NFTMetadata[] allNfts;
        private string defaultAddress = "erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf";

        public override void Init(params object[] args)
        {
            base.Init(args);
            nftDestination.text = defaultAddress;
        }

        public void NFTBackButton()
        {
            DemoScript.Instance.LoadScreen(Screens.Connected);
        }

        //liked to a button to load all NFTs from the wallet
        public void LoadWalletNFTs()
        {
            status.text = "Start loading NFTs";
            ElrondUnityTools.Manager.LoadWalletNFTs(LoadNFTComplete);
        }



        /// <summary>
        /// Listener triggered when NFT metadata is loaded
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        /// <param name="allNfts">All metadata properties serialized as NFTMetadata type</param>
        private void LoadNFTComplete(OperationStatus operationStatus, string message, NFTMetadata[] allNfts)
        {
            status.text = operationStatus + " " + message;
            this.allNfts = allNfts;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                //after all metadata is loaded the NFTs will be displayed in a scroll view
                for (int i = 0; i < allNfts.Length; i++)
                {
                    DisplayNft(allNfts[i]);
                }
            }
            else
            {
                Debug.Log(operationStatus + " " + message);
            }
        }

        /// <summary>
        /// Populates the NFT display container on the screen with the desired NFT properties 
        /// </summary>
        /// <param name="nFTMetadata"></param>
        private void DisplayNft(NFTMetadata nFTMetadata)
        {
            NFTHolder holderScript = Instantiate(nftItem, nftsHolder).GetComponent<NFTHolder>();
            holderScript.gameObject.name = nFTMetadata.name;
            //store the collection and the nonce because they will be needed to send the NFT to another walled
            holderScript.Initialize(this, nFTMetadata.name, nFTMetadata.collection, nFTMetadata.nonce);
            //load and display the NFT thumbnail
            Manager.LoadImage(nFTMetadata.media[0].thumbnailUrl, holderScript.image, null);
        }


        /// <summary>
        /// Refresh the wallet NFTs after sending an NFT
        /// </summary>
        /// <param name="collectionIdentifier"></param>
        /// <param name="nonce"></param>
        public void RefreshNFTs(string collectionIdentifier, int nonce)
        {
            //remove the NFT that was sent from this list
            allNfts = allNfts.Where(cond => cond.collection != collectionIdentifier || cond.nonce != nonce).ToArray();
            Manager.RefreshAccount();
        }
    }
}
