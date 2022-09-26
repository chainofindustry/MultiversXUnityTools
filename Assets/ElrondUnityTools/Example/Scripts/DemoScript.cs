using Erdcsharp.Provider.Dtos;
using System.Linq;
using UnityEngine;
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

        private GameObject loadedScreen;
        private AccountDto connectedAccount;


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
            if (loadedScreen != null)
            {
                Destroy(loadedScreen);
            }
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


        #region APIUsageExamples
        public void Get()
        {
            string url = "https://devnet-api.elrond.com/accounts/erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf";
            ElrondUnityTools.Manager.GetRequest(url, CompleteMethod);
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
                          "\"version\":1," + 
                          "\"signature\":\"72ddcb105778051ea2a6f92b3869e2110d50f708708a2a3fe842014c062152c8aff78dae39868d97d25831915d3c60f4acfc749dfa8bdfa395f3769d2e231a05\"" +
                          "}";

            //Make the Post request 
            ElrondUnityTools.Manager.PostRequest(url, json, CompleteMethod);
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
        #endregion
    }
}