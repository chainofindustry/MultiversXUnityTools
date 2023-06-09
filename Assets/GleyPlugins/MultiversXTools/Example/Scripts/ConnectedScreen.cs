using MultiversXUnityTools;
using Mx.NET.SDK.Domain.Data.Account;
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
        private void AccountRefreshed(CompleteCallback<Account> result)
        {
            status.text = $"Account Refresh Complete {result.status} {result.errorMessage}";
            if (result.status == OperationStatus.Success)
            {
                //if operation was success update the display values
                RefreshAccount(result.data);
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
        private void PictureLoadComplete(CompleteCallback<Texture2D> result)
        {
            status.text = $"Picture Load Complete {result.status} {result.errorMessage}";

            if (result.status == OperationStatus.Success)
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
        private void CoverLoadComplete(CompleteCallback<Texture2D> result)
        {
            status.text = $"Cover Load Complete {result.status}   {result.errorMessage}";

            if (result.status == OperationStatus.Success)
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