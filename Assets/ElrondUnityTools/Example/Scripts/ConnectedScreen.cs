using Erdcsharp.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class ConnectedScreen : GenericUIScreen
    {
        public Text address;
        public Text ht;
        public Text egldValue;
        public Text status;
        public Image profilePicture;
        public Image banner;

        public override void Init(params object[] args)
        {
            base.Init(args);
            status.text = "Refresh account";
            ElrondUnityTools.Manager.RefreshAccount(AccountRefreshed);
            LoadProfileImages(DemoScript.Instance.GetConnectedAccount().Address.ToString());
        }

        private void AccountRefreshed(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            status.text = $"Account Refresh Complete {operationStatus} {message}";
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                RefreshAccount(DemoScript.Instance.GetConnectedAccount());
            }
        }

        private void LoadProfileImages(string address)
        {
            status.text = "Start Loading Images";
            ElrondUnityTools.Manager.LoadImage($"https://id.maiar.com/users/photos/profile/{address}", profilePicture, PictureLoadComplete);
            ElrondUnityTools.Manager.LoadImage($"https://id.maiar.com/users/photos/cover/{address}", banner, CoverLoadComplete2);
        }

        private void PictureLoadComplete(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            status.text = $"Picture Load Complete {operationStatus} {message}";

            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                profilePicture.color = Color.white;
            }
        }
        private void CoverLoadComplete2(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            status.text = $"Cover Load Complete {operationStatus} {message}";

            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                banner.color = Color.white;
            }
        }

        /// <summary>
        /// Refresh the address and the amount of tokens of the connected wallet 
        /// </summary>
        /// <param name="connectedAccount"></param>
        private void RefreshAccount(Account connectedAccount)
        {
            Debug.Log("RefreshAccount " + connectedAccount.Balance);
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

        //linked to the disconnect button in editor
        public void Disconnect()
        {
            ElrondUnityTools.Manager.Disconnect();
        }
    }
}