using ElrondUnityTools;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace ElrondUnityExamples
{
    public class NftsScreen : GenericUIScreen
    {
        public InputField nftDestination;
        public Text nftStatus;
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
            nftStatus.text = operationStatus + " " + message;
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
            StartCoroutine(LoadImage(nFTMetadata.media[0].thumbnailUrl, holderScript.image));
        }


        /// <summary>
        /// Load the NFT Thumbnail from the url
        /// </summary>
        /// <param name="imageURL"></param>
        /// <param name="displayComponent">image component to display the downloaded thumbnail picture</param>
        /// <returns></returns>
        private IEnumerator LoadImage(string imageURL, Image displayComponent)
        {
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageURL);
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    Texture2D imageTex = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                    Sprite newSprite = Sprite.Create(imageTex, new Rect(0, 0, imageTex.width, imageTex.height), new Vector2(.5f, .5f));
                    displayComponent.sprite = newSprite;
                    break;
                default:
                    Debug.LogError(webRequest.error);
                    break;
            }
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
        }
    }
}
