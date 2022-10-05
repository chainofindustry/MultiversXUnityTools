using Erdcsharp.Domain;
using Erdcsharp.Provider.Dtos;
using System;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class ConnectedScreen : GenericUIScreen
    {
        public Text address;
        public Text ht;
        public Text egldValue;
        public Image profilePicture;
        public Image banner;

        public override void Init(params object[] args)
        {
            base.Init(args);
            RefreshAccount(DemoScript.Instance.GetConnectedAccount());
            LoadProfileImages(DemoScript.Instance.GetConnectedAccount().Address);
        }

        private void LoadProfileImages(string address)
        {

            ElrondUnityTools.Manager.LoadImage("https://id.maiar.com/users/photos/profile/" + address, profilePicture, PictureLoadComplete1);
            ElrondUnityTools.Manager.LoadImage("https://id.maiar.com/users/photos/cover/" + address, banner, PictureLoadComplete2);
        }

        private void PictureLoadComplete1(ElrondUnityTools.OperationStatus status, string arg1)
        {
            if (status == ElrondUnityTools.OperationStatus.Complete)
            {
                profilePicture.color = UnityEngine.Color.white;
            }
        }
        private void PictureLoadComplete2(ElrondUnityTools.OperationStatus status, string arg1)
        {
            if (status == ElrondUnityTools.OperationStatus.Complete)
            {
                banner.color = UnityEngine.Color.white;
            }
        }

        /// <summary>
        /// Refresh the address and the amount of tokens of the connected wallet 
        /// </summary>
        /// <param name="connectedAccount"></param>
        private void RefreshAccount(AccountDto connectedAccount)
        {
            var amount = TokenAmount.From(connectedAccount.Balance);
            address.text = connectedAccount.Address;
            egldValue.text = "EGLD: " + amount.ToDenominated();
            ht.text = "HT: " + connectedAccount.Username;
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