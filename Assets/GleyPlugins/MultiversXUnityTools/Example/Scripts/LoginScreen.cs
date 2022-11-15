using MultiversXUnityTools;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversXUnityExamples
{
    public class LoginScreen : GenericUIScreen
    {
        public Image qrImage;
        public GameObject warning;


        public override void Init(params object[] args)
        {
            //when this screen is active automatically call the connect method
            DemoScript.Instance.Connect(qrImage);

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
    }
}