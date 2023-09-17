using Mx.NET.SDK.Domain.Data.Accounts;
using UnityEngine;
using UnityEngine.UI;
using Network = Mx.NET.SDK.Configuration.Network;

namespace MultiversX.UnityTools.Examples
{
    public class LoginScreen : GenericUIScreen
    {
        public Image qrImage;
        public GameObject warning;
        public GameObject loginButton;
        public GameObject loading;

        public override void Init(params object[] args)
        {
            loginButton.SetActive(false);
            qrImage.gameObject.SetActive(false);

            //display warning is selected API is Mainnet 
            AppSettings apiSettings = API.GetApiSettings();
            if (apiSettings.selectedNetwork == Network.MainNet)
            {
                warning.SetActive(true);
            }
            else
            {
                warning.SetActive(false);
            }

            //when this screen is active automatically call the connect method
            API.Connect(OnConnected,DemoScript.Instance.OnDisconnected, OnSessionConnected, qrImage);
            
           
        }

        private void OnSessionConnected(string arg0)
        {
            loginButton.SetActive(true);
            qrImage.gameObject.SetActive(true);
            loading.SetActive(false);
        }


        //linked to the login button in editor
        public void Login()
        {
            API.DeepLinkLogin();
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
                loading.SetActive(false);
                warning.SetActive(true);
                warning.GetComponent<Text>().text = result.errorMessage;
            }
        }
    }
}