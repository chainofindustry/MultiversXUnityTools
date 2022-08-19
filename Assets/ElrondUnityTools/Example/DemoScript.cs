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
    enum Screens
    {
        Home,
        Login,
        Connected,
        NFT,
        SC
    }

    public class DemoScript : MonoBehaviour
    {
        public GameObject homeScreen;
        public GameObject loginScreen;
        public GameObject connectedScreen;
        public GameObject nftScreen;
        public GameObject scScreen;

        private Screens currentScreen;

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

        //sc
        public InputField scAddress;
        public InputField method;
        public InputField param;
        public Text scResultText;

        public string defaultScAddress = "erd1qqqqqqqqqqqqqpgqm6q54xrsrnynwjhyn53lpezm4x87zeth0eqqggct6q";
        public string defaultFuncName = "getSum";
        public string[] args;

        //set default values for everything
        private void Start()
        {
            currentScreen = Screens.Home;
            RefreshButtons();
            destination.text = nftDestination.text = defaultAddress;
            message.text = defaultMessage;
            amount.text = egld.ToString();
            esdtAmount.text = egld.ToString();
            PopulateDropDown();
            status.text = "";
            scAddress.text = defaultScAddress;
            method.text = defaultFuncName;
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
            currentScreen = Screens.Login;
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
            currentScreen = Screens.Connected;
            RefreshButtons();
        }


        /// <summary>
        /// Triggered when wallet disconnected
        /// </summary>
        private void OnDisconnected()
        {
            address.text = "-";
            status.text = "";
            currentScreen = Screens.Home;
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
        /// <param name="operationStatus">Completed, In progress or Error</param>
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
            homeScreen.SetActive(false);
            loginScreen.SetActive(false);
            connectedScreen.SetActive(false);
            nftScreen.SetActive(false);
            scScreen.SetActive(false);

            switch (currentScreen)
            {
                case Screens.Home:
                    homeScreen.SetActive(true);
                    break;
                case Screens.Login:
                    loginScreen.SetActive(true);
                    break;
                case Screens.Connected:
                    connectedScreen.SetActive(true);
                    break;
                case Screens.NFT:
                    nftScreen.SetActive(true);
                    break;
                case Screens.SC:
                    scScreen.SetActive(true);
                    break;
            }
        }

        #region NFTs

        //linked to the UI button to open the NFT screen
        public void ShowNFTScreen()
        {
            currentScreen = Screens.NFT;
            RefreshButtons();
        }

        public void NFTBackButton()
        {
            currentScreen = Screens.Connected;
            RefreshButtons();
        }


        //liked to a button to load all nfts from the wallet
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
        private void LoadNFTComplete(ElrondUnityTools.OperationStatus operationStatus, string message, ElrondUnityTools.NFTMetadata[] allNfts)
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
        private void DisplayNft(ElrondUnityTools.NFTMetadata nFTMetadata)
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

        #endregion

        #region SC
        public void ShowSCScreen()
        {
            currentScreen = Screens.SC;
            RefreshButtons();
        }

        public void SCBack()
        {
            currentScreen = Screens.Connected;
            RefreshButtons();
        }

        public void ExecuteQuery()
        {
            ElrondUnityTools.Manager.MakeSCQuery(scAddress.text, method.text, new string[] { param.text }, QueryComplete);
        }

        private void QueryComplete(ElrondUnityTools.OperationStatus operationStatus, string message, ElrondUnityTools.SCData data)
        {
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                scResultText.text = "Raw data: \n" + Newtonsoft.Json.JsonConvert.SerializeObject(data);
                string encodedText = data.returnData[0];

                var result = Erdcsharp.Domain.Helper.Converter.ToBigInteger(Convert.FromBase64String(encodedText));

                scResultText.text += "\n\n Current sum: " + result;
                Debug.Log(result);
            }
            else
            {
                Debug.LogError(message);
                scResultText.text = message;
            }
        }
        #endregion

    }
}