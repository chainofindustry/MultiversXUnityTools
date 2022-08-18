using Erdcsharp.Domain;
using Erdcsharp.Provider.Dtos;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class DemoScript : MonoBehaviour
    {
        public GameObject homeScreen;
        public GameObject loginScreen;
        public GameObject connectedScreen;
        public GameObject nftScreen;

        //login
        public Image qrImage;
        public GameObject loginButton;
        public Text loginStatus;

        //connected
        public Text address;
        public Text status;
        public InputField destination, nftDestination;
        public InputField amount;
        public InputField message;
        public GameObject disconnectButton;
        public GameObject transactionButton;

        private bool loginInProgress;
        private bool showNFTs;
        private AccountDto connectedAccount;
        private string txHash;

        private string defaultAddress = "erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf";
        private string defaultMessage = "You see this?";
        private double egld = 0.001;

        //ESDT
        public InputField esdtAmount;
        public Dropdown esdtTokenDropdown;

        //nfts
        public Transform nftsHolder;
        public GameObject nftItem;
        public Text nftStatus;
        ElrondUnityTools.NFTMetadata[] allNfts;

        //set default values for everything
        private void Start()
        {
            RefreshButtons();
            destination.text = nftDestination.text = defaultAddress;
            message.text = defaultMessage;
            amount.text = egld.ToString();
            esdtAmount.text = egld.ToString();
            PopulateDropDown();
            status.text = "";
        }


        //linked to the login button in editor
        public void Login()
        {
            ElrondUnityTools.Manager.DeepLinkLogin();
        }


        //linked to the login options button in editor
        public void LoginOptions()
        {
            ElrondUnityTools.Manager.Connect(OnConnected, OnDisconnected, qrImage);
            loginInProgress = true;
            RefreshButtons();
        }


        //linked to the disconnect button in editor
        public void Disconnect()
        {
            ElrondUnityTools.Manager.Disconnect();
        }


        //linked to the send transaction button in editor
        public void SendTransaction()
        {
            status.text = "";

            //should verify first if destination, amount and message are in the correct format
            ElrondUnityTools.Manager.SendTransaction(destination.text, amount.text, message.text, SigningStatusListener);
        }


        //linked to Send ESDT Transaction button
        public void SendESDTTransaction()
        {
            // get the drop down state and determine the ESDT token to transfer
            ElrondUnityTools.ESDTToken selectedToken = ElrondUnityTools.SupportedESDTTokens.USDC;
            switch (esdtTokenDropdown.options[esdtTokenDropdown.value].text)
            {
                case "USDC":
                    selectedToken = ElrondUnityTools.SupportedESDTTokens.USDC;
                    break;
                case "WEB":
                    selectedToken = ElrondUnityTools.SupportedESDTTokens.WEB;
                    break;
            }
            ElrondUnityTools.Manager.SendESDTTransaction(destination.text, selectedToken, esdtAmount.text, SigningStatusListener);
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }


        /// <summary>
        /// Populate drop down list with available ESDT tokens
        /// </summary>
        void PopulateDropDown()
        {
            esdtTokenDropdown.options.Clear();
            esdtTokenDropdown.options.Add(new Dropdown.OptionData() { text = ElrondUnityTools.SupportedESDTTokens.USDC.name });
            esdtTokenDropdown.options.Add(new Dropdown.OptionData() { text = ElrondUnityTools.SupportedESDTTokens.WEB.name });
        }


        /// <summary>
        /// Triggered when Maiar app connected
        /// </summary>
        /// <param name="connectedAccount">A class containing informations about the connected wallet</param>
        private void OnConnected(AccountDto connectedAccount)
        {
            this.connectedAccount = connectedAccount;
            RefreshAccount(connectedAccount);
            RefreshButtons();
        }


        /// <summary>
        /// Triggered when wallet disconnected
        /// </summary>
        private void OnDisconnected()
        {
            address.text = "-";
            status.text = "";
            RefreshButtons();
        }


        /// <summary>
        /// Refresh the address and the amount of tokens of the connected wallet 
        /// </summary>
        /// <param name="connectedAccount"></param>
        private void RefreshAccount(AccountDto connectedAccount)
        {
            var amount = TokenAmount.From(connectedAccount.Balance);
            address.text = connectedAccount.Address + "\n EGLD: " + amount.ToDenominated();
            if (!string.IsNullOrEmpty(connectedAccount.Username))
            {
                address.text += "\nHT: " + connectedAccount.Username;
            }
        }


        /// <summary>
        /// Track the status of the signing transaction
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message">if the operation status is complete, the message is the txHash</param>
        private void SigningStatusListener(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            status.text = operationStatus + " " + message;
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
            status.text = operationStatus + " " + message;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                Debug.Log(message);
                if (message == "pending")
                {
                    ElrondUnityTools.Manager.CheckTransactionStatus(txHash, BlockchainTransactionListener, 1);
                }
                else
                {
                    if (message == "success")
                    {
                        RefreshAccount(connectedAccount);
                    }
                }
            }
        }


        /// <summary>
        /// Set the required UI
        /// </summary>
        void RefreshButtons()
        {
            if (ElrondUnityTools.Manager.IsWalletConnected() == false)
            {
                if (loginInProgress == true)
                {
                    //show login UI
                    homeScreen.SetActive(false);
                    loginScreen.SetActive(true);
                    connectedScreen.SetActive(false);
                    nftScreen.SetActive(false);
                }
                else
                {

                    //show home screen UI
                    homeScreen.SetActive(true);
                    loginScreen.SetActive(false);
                    connectedScreen.SetActive(false);
                    nftScreen.SetActive(false);
                }
            }
            else
            {
                if (showNFTs == false)
                {
                    //show connected screen UI
                    homeScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    connectedScreen.SetActive(true);
                    nftScreen.SetActive(false);
                }
                else
                {
                    //show connected screen UI
                    homeScreen.SetActive(false);
                    loginScreen.SetActive(false);
                    connectedScreen.SetActive(false);
                    nftScreen.SetActive(true);
                }
            }
        }

        #region NFTs
        public void ShowNFTScreen()
        {
            showNFTs = true;
            RefreshButtons();
        }


        public void LoadWalletNFTs()
        {
            ElrondUnityTools.Manager.LoadWalletNFTs(LoadNFTComplete);
        }

        private void LoadNFTComplete(ElrondUnityTools.OperationStatus status, string message, ElrondUnityTools.NFTMetadata[] allNfts)
        {
            nftStatus.text = status + " " + message;
            this.allNfts = allNfts;
            if (status == ElrondUnityTools.OperationStatus.Complete)
            {
                //after all metadata is loaded th nfts will be displayed in a scroll view
                for (int i = 0; i < allNfts.Length; i++)
                {
                    DisplayNft(allNfts[i]);
                }
            }
            else
            {
                Debug.Log(status + " " + message);
            }
        }

        private void DisplayNft(ElrondUnityTools.NFTMetadata nFTMetadata)
        {
            NFTHolder holderScript = Instantiate(nftItem, nftsHolder).GetComponent<NFTHolder>();
            holderScript.gameObject.name = nFTMetadata.name;
            holderScript.Initialize(this, nFTMetadata.name, nFTMetadata.collection, nFTMetadata.nonce);
            //load and display the NFT thumbnail
            StartCoroutine(LoadImage(nFTMetadata.media[0].thumbnailUrl, holderScript.image));
        }

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

        public void RefreshNFTs(string collectionIdentifier, int nonce)
        {
            //remove the NFT that was sent from this list
            allNfts = allNfts.Where(cond => cond.collection != collectionIdentifier || cond.nonce != nonce).ToArray();
        }

        #endregion
    }
}