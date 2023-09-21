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
    }
}