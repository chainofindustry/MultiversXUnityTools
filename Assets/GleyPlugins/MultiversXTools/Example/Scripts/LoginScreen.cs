using MultiversXUnityTools;
using Mx.NET.SDK.Domain.Data.Account;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversXUnityExamples
{
    public class LoginScreen : GenericUIScreen
    {
        public Image qrImage;
        public GameObject warning;
        public GameObject loginButton;

        public override void Init(params object[] args)
        {
            loginButton.SetActive(false);   
            //when this screen is active automatically call the connect method
            Manager.Connect(OnConnected,DemoScript.Instance.OnDisconnected, OnSessionConnected, qrImage);
            
            //display warning is selected API is Mainnet 
            APISettings apiSettings = Manager.GetApiSettings();
            if (apiSettings.selectedAPIName == SupportedAPIs.MultiversXApiMainnet.ToString())
            {
                warning.SetActive(true);
            }
            else
            {
                warning.SetActive(false);
            }
        }

        private void OnSessionConnected(string arg0)
        {
            loginButton.SetActive(true);
        }


        //linked to the login button in editor
        public void Login()
        {
            Manager.DeepLinkLogin();
        }


        //linked to the back button in editor
        public void BackButton()
        {
            DemoScript.Instance.LoadScreen(Screens.Home);
        }

        /// <summary>
        /// Triggered when xPortal app connected
        /// </summary>
        /// <param name="connectedAccount">A class containing informations about the connected wallet</param>
        private void OnConnected(CompleteCallback<Account> result)
        {
            //load the connected screen
            if (result.status == OperationStatus.Success)
            {
                DemoScript.Instance.LoadScreen(Screens.Connected, result.data);
            }
            else
            {
                Debug.LogError(result.errorMessage);
                //reload
                DemoScript.Instance.LoadScreen(Screens.Login);
            }
        }
    }
}