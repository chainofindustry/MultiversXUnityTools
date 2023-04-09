using Erdcsharp.Domain;
using MultiversXUnityTools;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversXUnityExamples
{
    public class ConnectedScreen : GenericUIScreen
    {
        public Text address;
        public Text ht;
        public Text egldValue;
        public Text status; // used to display on screen messages
        public Image profilePicture;
        public Image banner;

        public override void Init(params object[] args)
        {
            base.Init(args);
            status.text = "Refresh account";
            //refresh the account balance
            Manager.RefreshAccount(AccountRefreshed);
            //load profile picture and cover photo from xPortal
            LoadProfileImages(Manager.GetConnectedAccount().Address.ToString());
        }


        /// <summary>
        /// Callback triggered when refresh account operation is complete
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        private void AccountRefreshed(OperationStatus operationStatus, string message)
        {
            status.text = $"Account Refresh Complete {operationStatus} {message}";
            if (operationStatus == OperationStatus.Complete)
            {
                //if operation was success update the display values
                RefreshAccount(Manager.GetConnectedAccount());
            }
        }


        /// <summary>
        /// Load profile and cover photos from xPortal
        /// </summary>
        /// <param name="address"></param>
        private void LoadProfileImages(string address)
        {
            status.text = "Start Loading Images";
            Manager.LoadImage($"https://id-api.multiversx.com/users/photos/profile/{address}", profilePicture, PictureLoadComplete);
            Manager.LoadImage($"https://id-api.multiversx.com/users/photos/cover/{address}", banner, CoverLoadComplete);
        }


        /// <summary>
        /// Callback triggered when picture load is complete
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        private void PictureLoadComplete(OperationStatus operationStatus, string message)
        {
            status.text = $"Picture Load Complete {operationStatus} {message}";

            if (operationStatus == OperationStatus.Complete)
            {
                //color of picture holder image was set to the background color by default in case that picture is null to look better
                //if picture was loaded set color to white to display the picture properly
                profilePicture.color = Color.white;
            }
        }


        /// <summary>
        /// Callback triggered when cover load is complete
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        private void CoverLoadComplete(OperationStatus operationStatus, string message)
        {
            status.text = $"Cover Load Complete {operationStatus} {message}";

            if (operationStatus == OperationStatus.Complete)
            {
                //color of picture holder image was set to the background color by default in case that picture is null to look better
                //if picture was loaded set color to white to display the picture properly
                banner.color = Color.white;
            }
        }


        /// <summary>
        /// Refresh the address and the amount of tokens of the connected wallet 
        /// </summary>
        /// <param name="connectedAccount"></param>
        private void RefreshAccount(Account connectedAccount)
        {
            var amount = connectedAccount.Balance;
            address.text = connectedAccount.Address.ToString();
            egldValue.text = "EGLD: " + amount.ToDenominated();
            ht.text = "HT: " + connectedAccount.UserName;
        }


        //linked to the UI button to open the Transactions screen
        public void ShowTransactions()
        {
            DemoScript.Instance.LoadScreen(Screens.Transactions);
        }


        //linked to the UI button to open the SC screen
        public void ShowSCScreen()
        {
            DemoScript.Instance.LoadScreen(Screens.SC);
        }


        //linked to the UI button to open the NFT screen
        public void ShowNFTScreen()
        {
            DemoScript.Instance.LoadScreen(Screens.NFT);
        }


        public void SignMessage()
        {
            DemoScript.Instance.LoadScreen(Screens.SignMessage);
        }

      

        //linked to the disconnect button in editor
        public void Disconnect()
        {
            Manager.Disconnect();
        }
    }
}