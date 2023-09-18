using Mx.NET.SDK.Provider.Dtos.API.Accounts;
using Mx.NET.SDK.Provider.Dtos.Gateway.Transactions;
using System.Linq;
using UnityEngine;

namespace MultiversX.UnityTools.Examples
{
    //list of available UI screens
    public enum Screens
    {
        Home,
        Login,
        Connected,
        NFT,
        SC,
        Transactions,
        SignMessage
    }


    //custom class to associate the prefab with the Screens enum
    [System.Serializable]
    public struct UIScreen
    {
        public Screens screenName;
        public GameObject screenPrefab;
    }


    //A simple navigation script to move through different UI screens
    public class DemoScript : MonoBehaviour
    {
        public UIScreen[] allScreens;
        public Transform canvas;

        private GameObject loadedScreen;
        //private Account connectedAccount;

        //static instance to be able to access the script from UI scripts
        public static DemoScript Instance { get; private set; }

        //simple singleton pattern
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
            //set default screen to Home
            LoadScreen(Screens.Home);
        }


        /// <summary>
        /// Simple UI loading method
        /// </summary>
        /// <param name="newScreen"></param>
        /// <param name="args"></param>
        public void LoadScreen(Screens newScreen, params object[] args)
        {
            if (loadedScreen != null)
            {
                Destroy(loadedScreen);
            }
            //load the corresponding screen and automatically call the Init method
            loadedScreen = allScreens.FirstOrDefault(cond => cond.screenName == newScreen).screenPrefab;
            loadedScreen = Instantiate(loadedScreen, canvas);
            loadedScreen.GetComponent<GenericUIScreen>().Init(args);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            //test for custom Post and Get methods
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    Get();
            //    Post();
            //}
        }


       


        /// <summary>
        /// Triggered when wallet disconnected
        /// </summary>
        public void OnDisconnected()
        {
            //every time used disconnects go to home screen
            LoadScreen(Screens.Home);
        }


        #region APIUsageExamples
        /// <summary>
        /// An usage example for a generic Get call
        /// </summary>
        public void Get()
        {
            //string url = Manager.GetEndpointUrl(EndpointNames.GetAccount).Replace("{address}", "erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf");
            //Manager.GetRequest<AccountDto>(url, CompleteMethodGet);
        }


        /// <summary>
        /// Callback for Get method
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        private void CompleteMethodGet(CompleteCallback<AccountDto> result)
        {
            if (result.status == OperationStatus.Success)
            {
                Debug.Log(result.data.Address);
            }
            else
            {
                Debug.LogError(result.errorMessage);
            }
        }


        /// <summary>
        /// An usage example for a generic Post call
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        public void Post()
        {
            //construct the url
            string url = "https://devnet-gateway.multiversx.com/transaction/cost";

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
                          "\"version\":1," + 
                          "\"signature\":\"72ddcb105778051ea2a6f92b3869e2110d50f708708a2a3fe842014c062152c8aff78dae39868d97d25831915d3c60f4acfc749dfa8bdfa395f3769d2e231a05\"" +
                          "}";

            //Make the Post request 
            API.PostRequest<TransactionCostResponseDto>(url, json, CompleteMethodPost);
        }


        /// <summary>
        /// Callback for Post method
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        private void CompleteMethodPost(CompleteCallback<TransactionCostResponseDto> result)
        {
            if (result.status == OperationStatus.Success)
            {
                Debug.Log(result.data.TxGasUnits);
            }
            else
            {
                Debug.LogError(result.errorMessage);
            }
        }
        #endregion
    }
}