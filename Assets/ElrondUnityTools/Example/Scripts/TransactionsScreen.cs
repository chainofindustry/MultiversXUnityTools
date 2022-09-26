using ElrondUnityTools;
using Erdcsharp.Domain;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class TransactionsScreen : GenericUIScreen
    {
        public Text status;
        public InputField destination;
        public InputField amount;
        public InputField message;
        public InputField esdtAmount;
        public Dropdown esdtTokenDropdown;
        public Transform tokenParent;
        public GameObject tokenHolder;



        private string defaultAddress = "erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf";
        private string defaultMessage = "You see this?";
        private double egld = 0.001;
        private string txHash;

        public override void Init(params object[] args)
        {
            base.Init(args);
            destination.text = defaultAddress;
            message.text = defaultMessage;
            amount.text = egld.ToString();
            PopulateDropDown();
            status.text = "Start loading tokens";
            Manager.LoadAllTokens(TokensLoaded);
        }

        private void TokensLoaded(OperationStatus operationStatus, string message, TokenMetadata[] allTokens)
        {
            status.text = operationStatus + " " + message;
            if (operationStatus == OperationStatus.Complete)
            {
                PopulateUI(allTokens);
            }
        }

        void PopulateUI(TokenMetadata[] allTokens)
        {
            while (tokenParent.childCount > 0)
            {
                DestroyImmediate(tokenParent.GetChild(0).gameObject);
            }

            for (int i = 0; i < allTokens.Length; i++)
            {
                TokenHolder script = Instantiate(tokenHolder, tokenParent).GetComponent<TokenHolder>();
                script.value.text = (BigInteger.Parse(allTokens[i].balance) / BigInteger.Pow(10, allTokens[i].decimals)).ToString("N2");
                script.tokenName.text = allTokens[i].name;
                if (allTokens[i].assets != null)
                {
                    if (!string.IsNullOrEmpty(allTokens[i].assets.pngUrl))
                    {
                        Manager.LoadImage(allTokens[i].assets.pngUrl, script.tokenImage);
                    }
                    else
                    {
                        Destroy(script.tokenImage.transform.parent.gameObject);
                    }
                }
                else
                {
                    Destroy(script.tokenImage.transform.parent.gameObject);
                }
            }
        }

        public void BackButton()
        {
            DemoScript.Instance.LoadScreen(Screens.Connected);
        }

        //linked to the send transaction button in editor
        public void SendTransaction()
        {
            status.text = "";

            //should verify first if destination, amount and message are in the correct format
            ElrondUnityTools.Manager.SendEGLDTransaction(destination.text, amount.text, message.text, SigningStatusListener);
        }


        //linked to Send ESDT Transaction button
        public void SendESDTTransaction()
        {
            // get the drop down state and determine the ESDT token to transfer
            ElrondUnityTools.ESDTToken selectedToken = ElrondUnityTools.SupportedESDTTokens.USDC;
            switch (esdtTokenDropdown.options[esdtTokenDropdown.value].text)
            {
                case "USDC":
                    selectedToken = ElrondUnityTools.SupportedESDTTokens.USDC;
                    break;
                case "WEB":
                    selectedToken = ElrondUnityTools.SupportedESDTTokens.WEB;
                    break;
            }
            ElrondUnityTools.Manager.SendESDTTransaction(destination.text, selectedToken, esdtAmount.text, SigningStatusListener);
        }

        void PopulateDropDown()
        {
            Debug.Log(esdtTokenDropdown);
            esdtTokenDropdown.options.Clear();
            esdtTokenDropdown.options.Add(new Dropdown.OptionData() { text = ElrondUnityTools.SupportedESDTTokens.USDC.name });
            esdtTokenDropdown.options.Add(new Dropdown.OptionData() { text = ElrondUnityTools.SupportedESDTTokens.WEB.name });
        }


        /// <summary>
        /// Track the status of the signing transaction
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">if the operation status is complete, the message is the txHash</param>
        private void SigningStatusListener(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            status.text = operationStatus + " " + message;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                txHash = message;
                Debug.Log("Tx Hash: " + txHash);
                ElrondUnityTools.Manager.CheckTransactionStatus(txHash, BlockchainTransactionListener, 1);
            }
            if (operationStatus == ElrondUnityTools.OperationStatus.Error)
            {
                //do something
            }
        }

        /// <summary>
        /// Listener for the transaction status response
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void BlockchainTransactionListener(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            status.text = operationStatus + " " + message;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                Debug.Log(message);
                if (message == "pending")
                {
                    ElrondUnityTools.Manager.CheckTransactionStatus(txHash, BlockchainTransactionListener, 1);
                }
                else
                {
                    if (message == "success")
                    {
                        ElrondUnityTools.Manager.RefreshAccount(RefreshDone);
                        status.text = "Success -> Refreshing tokens";
                        
                    }
                }
            }
        }

        private void RefreshDone()
        {
            Manager.LoadAllTokens(TokensLoaded);
        }
    }
}
