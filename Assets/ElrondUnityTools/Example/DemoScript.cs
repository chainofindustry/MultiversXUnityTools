using Erdcsharp.Domain;
using Erdcsharp.Provider.Dtos;
using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class DemoScript : MonoBehaviour
    {
        public GameObject homeScreen;
        public GameObject loginScreen;
        public GameObject connectedScreen;

        //login
        public Image qrImage;
        public GameObject loginButton;
        public Text loginStatus;

        //connected
        public Text address;
        public Text status;
        public GameObject disconnectButton;
        public GameObject transactionButton;

        bool loginInProgress;

        private void Start()
        {
            RefreshButtons();
        }


        private void OnConnected(AccountDto connectedAccount)
        {
            Debug.Log("OnConnected");
            RefreshAccount(connectedAccount);
            RefreshButtons();
        }

        private void OnDisconnected()
        {
            Debug.Log("OnDisconnected");
            address.text = "-";
            status.text = "";
            RefreshButtons();
        }


        void RefreshAccount(AccountDto connectedAccount)
        {
            Debug.Log("refresh account");
            var amount = TokenAmount.From(connectedAccount.Balance);

            address.text = connectedAccount.Address + "\n EGLD: " + amount.ToDenominated();
            if (!string.IsNullOrEmpty(connectedAccount.Username))
            {
                address.text += "\nHT: " + connectedAccount.Username;
            }
        }

        public void Login()
        {
            ElrondUnityTools.Manager.DeepLinkLogin();
        }


        public void LoginOptions()
        {
            ElrondUnityTools.Manager.Connect(OnConnected, OnDisconnected, qrImage);
            loginInProgress = true;
            RefreshButtons();
        }

        public void Disconnect()
        {
            ElrondUnityTools.Manager.Disconnect();
        }


        public void SendTransaction()
        {
            status.text = "";
            ElrondUnityTools.Manager.SendTransaction("", "", "", StatusListener);
        }

        private void StatusListener(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            status.text = operationStatus + " " + message;
        }

        void RefreshButtons()
        {
            if (ElrondUnityTools.Manager.IsWalletConnected() == false)
            {
                if (loginInProgress == true)
                {
                    homeScreen.SetActive(false);
                    loginScreen.SetActive(true);
                    connectedScreen.SetActive(false);
                }
                else
                {
                    homeScreen.SetActive(true);
                    loginScreen.SetActive(false);
                    connectedScreen.SetActive(false);
                }
            }
            else
            {
                homeScreen.SetActive(false);
                loginScreen.SetActive(false);
                connectedScreen.SetActive(true);
            }
        }
    }
}