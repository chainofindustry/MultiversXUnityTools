using Erdcsharp.Domain;
using MultiversXUnityTools;
using System;
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
        /// Triggered when Maiar app connected
        /// </summary>
        /// <param name="connectedAccount">A class containing informations about the connected wallet</param>
        private void OnConnected(Account connectedAccount, string error)
        {
            //load the connected screen
            if (connectedAccount != null)
            {
                DemoScript.Instance.LoadScreen(Screens.Connected, connectedAccount);
            }
            else
            {
                Debug.LogError(error);
                //reload
                DemoScript.Instance.LoadScreen(Screens.Login);
            }
        }
    }
}