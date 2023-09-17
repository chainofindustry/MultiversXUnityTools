using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversX.UnityTools.Examples
{
    public class NftsScreen : GenericUIScreen
    {
        public InputField nftDestination;
        public Text status;
        public Transform nftsHolder;
        public GameObject nftItem;

        private NFTMetadata[] allNfts;
        private string defaultAddress = "erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf";
        private int downloaded = 0;
        private int total = 0;


        public override void Init(params object[] args)
        {
            base.Init(args);
            //set default address in the input field
            nftDestination.text = defaultAddress;
        }


        //linked to the beck button in editor
        public void NFTBackButton()
        {
            DemoScript.Instance.LoadScreen(Screens.Connected);
        }


        //liked to a button to load all NFTs from the wallet
        public void LoadWalletNFTs()
        {
            status.text = "Start loading NFTs";
            API.LoadWalletNFTs(LoadNFTComplete);
        }


        /// <summary>
        /// Listener triggered when NFT metadata is loaded
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        /// <param name="allNfts">All metadata properties serialized as NFTMetadata type</param>
        private void LoadNFTComplete(CompleteCallback<NFTMetadata[]> result)
        {
            status.text = result.status + " " + result.errorMessage;
            allNfts = result.data;
            if (result.status == OperationStatus.Success)
            {
                //after all metadata is loaded the NFTs will be displayed in a scroll view
                StartCoroutine(LoadNFTs(allNfts,10));
            }
            else
            {
                Debug.Log(result.errorMessage);
            }
        }


        /// <summary>
        /// Load NFTs async
        /// </summary>
        /// <param name="allNfts">array with all NFT metadata</param>
        /// <param name="paralelDownloads">number of NFTs to be downloaded simultaneous</param>
        /// <returns></returns>
        IEnumerator LoadNFTs(NFTMetadata[] allNfts, int paralelDownloads)
        {
            if (allNfts.Length == 0)
            {
                status.text = "No NFTs found";
            }
            else
            {
                status.text = "Start downloading NFTs";
            }
            downloaded = 0;
            total = allNfts.Length;
            int started = 0;
            while (started < total)
            {
                if (started - downloaded < paralelDownloads)
                {
                    DisplayNft(allNfts[started]);
                    started++;
                }
                yield return null;
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
            API.LoadImage(nFTMetadata.media[0].thumbnailUrl, holderScript.image, NFTLoaded);
        }


        /// <summary>
        /// Callback when an NFT image is downloaded
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        private void NFTLoaded(CompleteCallback<Texture2D> result)
        {
            downloaded++;
            status.text = $"Downloaded {downloaded}/{total}";
            if(downloaded==total)
            {
                status.text = "Downloaded Complete";
            }
        }


        /// <summary>
        /// Refresh the wallet NFTs after sending an NFT
        /// </summary>
        /// <param name="collectionIdentifier"></param>
        /// <param name="nonce"></param>
        public void RefreshNFTs(string collectionIdentifier, ulong nonce)
        {
            //remove the NFT that was sent from this list
            allNfts = allNfts.Where(cond => cond.collection != collectionIdentifier || cond.nonce != nonce).ToArray();
            API.RefreshAccount();
        }
    }
}
