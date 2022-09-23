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
    public enum Screens
    {
        Home,
        Login,
        Connected,
        NFT,
        SC,
        Transactions
    }

    [System.Serializable]
    public struct UIScreen
    {
        public Screens screenName;
        public GameObject screenPrefab;
    }

    public class DemoScript : MonoBehaviour
    {
        public UIScreen[] allScreens;
        public Transform canvas;
        GameObject loadedScreen;

        public GameObject loginScreen;
        public GameObject connectedScreen;
        public GameObject nftScreen;
        public GameObject scScreen;

        //connected


        public Text status;
        public InputField destination;

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

        //NFTs


        




        string[] args;

        public static DemoScript Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }


        //set default values for everything
        private void Start()
        {
            LoadScreen(Screens.Home);
            //RefreshButtons();
            //destination.text = nftDestination.text = defaultAddress;
            //message.text = defaultMessage;
            //amount.text = egld.ToString();
            //esdtAmount.text = egld.ToString();
            //PopulateDropDown();
            //status.text = "";
           
        }

        public void Connect(Image qrImage)
        {
            ElrondUnityTools.Manager.Connect(OnConnected, OnDisconnected, qrImage);
        }

        public AccountDto GetConnectedAccount()
        {
            return connectedAccount;
        }


        public void LoadScreen(Screens newScreen, params object[] args)
        {
            Debug.Log("aaaaaaaaa"+newScreen);
            if (loadedScreen != null)
            {
                Destroy(loadedScreen);
            }
            loadedScreen = allScreens.FirstOrDefault(cond => cond.screenName == newScreen).screenPrefab;
            loadedScreen = Instantiate(loadedScreen, canvas);
            loadedScreen.GetComponent<GenericUIScreen>().Init(args);
        }


       


        //linked to the send transaction button in editor
        public void SendTransaction()
        {
            status.text = "";

            //should verify first if destination, amount and message are in the correct format
            ElrondUnityTools.Manager.SendEGLDTransaction(destination.text, amount.text, message.text, SigningStatusListener);
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
            Debug.Log(esdtTokenDropdown);
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
            LoadScreen(Screens.Connected, connectedAccount);
        }


        /// <summary>
        /// Triggered when wallet disconnected
        /// </summary>
        private void OnDisconnected()
        {
            LoadScreen(Screens.Home);
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
                        //RefreshAccount(connectedAccount);
                    }
                }
            }
        }


        #region API
        public void Get()
        {
            string url = "https://devnet-api.elrond.com/accounts/erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf";
            ElrondUnityTools.Manager.GetRequest(url, CompleteMethod);
        }

        private void CompleteMethod(ElrondUnityTools.OperationStatus operationStatus, string message, string resultJson)
        {
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                Debug.Log(resultJson);
            }
            else
            {
                Debug.LogError(message + " " + resultJson);
            }
        }

        public void Post()
        {
            //construct the url
            string url = "https://devnet-gateway.elrond.com/transaction/cost";

            //construct the params as a json string
            string json = "{" +
                          "\"nonce\":0," +
                          "\"sender\":\"erd1lgp3ezf2wfkejnu0sm5y9g4x3ad05gr8lfc0g69vvdwwj0wjv0gscv2w4s\"," +
                          "\"receiver\":\"erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf\"," +
                          "\"value\":\"1000000000000000\"," +
                          "\"gasPrice\":1000000000," +
                          "\"gasLimit\":89000," +
                          "\"data\":\"WW91IHNlZSB0aGlzPw==\"," +
                          "\"chainId\":\"D\"," +
                          "\"version\":1," + "\"signature\":\"72ddcb105778051ea2a6f92b3869e2110d50f708708a2a3fe842014c062152c8aff78dae39868d97d25831915d3c60f4acfc749dfa8bdfa395f3769d2e231a05\"" +
                          "}";

            //Make the Post request 
            ElrondUnityTools.Manager.PostRequest(url, json, CompleteMethod);
        }
        #endregion
    }
}