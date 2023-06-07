using MultiversXUnityTools;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain.Data.Token;
using Mx.NET.SDK.Provider.Dtos.API.Account;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversXUnityExamples
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
        ESDT selectedToken;

        private string defaultAddress = "erd1jza9qqw0l24svfmm2u8wj24gdf84hksd5xrctk0s0a36leyqptgs5whlhf";
        private string defaultMessage = "You see this?";
        private double egld = 0.001;
        int transactionsToProcess;

        public override void Init(params object[] args)
        {
            base.Init(args);
            //load default values
            destination.text = defaultAddress;
            message.text = defaultMessage;
            amount.text = egld.ToString();
            //populate tokens available to transfer
            PopulateDropDown();
            status.text = "Start loading tokens";
            //load all wallet tokens
            Manager.LoadAllTokens(TokensLoaded);
        }


        /// <summary>
        /// Callback triggered when token metadata finished loading
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        /// <param name="allTokens"></param>
        private void TokensLoaded(OperationStatus operationStatus, string message, TokenMetadata[] allTokens)
        {
            status.text = $"Tokens Loaded status: {operationStatus}. Message: {message}";
            if (operationStatus == OperationStatus.Complete)
            {
                //Add tokens to UI
                PopulateUI(allTokens);
            }
        }

        /// <summary>
        /// Add token value, name and identifier to UI
        /// </summary>
        /// <param name="allTokens"></param>
        void PopulateUI(TokenMetadata[] allTokens)
        {
            while (tokenParent.childCount > 0)
            {
                DestroyImmediate(tokenParent.GetChild(0).gameObject);
            }

            for (int i = 0; i < allTokens.Length; i++)
            {
                TokenHolder script = Instantiate(tokenHolder, tokenParent).GetComponent<TokenHolder>();
                //display token value
                script.value.text = (BigInteger.Parse(allTokens[i].balance) / BigInteger.Pow(10, allTokens[i].decimals)).ToString("N2");
                script.tokenName.text = allTokens[i].name;
                if (allTokens[i].assets != null)
                {
                    if (!string.IsNullOrEmpty(allTokens[i].assets.pngUrl))
                    {
                        //load token icon
                        Manager.LoadImage(allTokens[i].assets.pngUrl, script.tokenImage, null);
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


        //linked in editor to back button
        public void BackButton()
        {
            DemoScript.Instance.LoadScreen(Screens.Connected);
        }


        //linked to the send transaction button in editor
        public void SendTransaction()
        {
            status.text = "Send Transaction";

            //should verify first if destination, amount and message are in the correct format
            Manager.SendEGLDTransaction(destination.text, amount.text, message.text, SigningStatusListener);
        }


        //linked to Send ESDT Transaction button
        public void SendESDTTransaction()
        {
            // get the drop down state and determine the ESDT token to transfer
            selectedToken = SupportedESDTTokens.USDC;
            switch (esdtTokenDropdown.options[esdtTokenDropdown.value].text)
            {
                case "USDC":
                    selectedToken = SupportedESDTTokens.USDC;
                    break;
                case "WEB":
                    selectedToken = SupportedESDTTokens.WEB;
                    break;
            }
            Manager.SendESDTTransaction(destination.text, selectedToken, esdtAmount.text, SigningStatusListener);
        }


        public void SendAllTransactions()
        {
            selectedToken = SupportedESDTTokens.USDC;
            switch (esdtTokenDropdown.options[esdtTokenDropdown.value].text)
            {
                case "USDC":
                    selectedToken = SupportedESDTTokens.USDC;
                    break;
                case "WEB":
                    selectedToken = SupportedESDTTokens.WEB;
                    break;
            }


            TransactionToSign[] transactions =
            {
                new TransactionToSign(destination.text, amount.text, message.text),
                new TransactionToSign(destination.text, selectedToken, esdtAmount.text)
            };
            Manager.SendMultipletrasactions(transactions, SigningStatusListener);
        }

        /// <summary>
        /// Add dropdown options
        /// </summary>
        void PopulateDropDown()
        {
            esdtTokenDropdown.options.Clear();
            esdtTokenDropdown.options.Add(new Dropdown.OptionData() { text = SupportedESDTTokens.USDC.Name });
            esdtTokenDropdown.options.Add(new Dropdown.OptionData() { text = SupportedESDTTokens.WEB.Name });
        }


        /// <summary>
        /// Track the status of the signing transaction
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">if the operation status is complete, the message is the txHash</param>
        private void SigningStatusListener(OperationStatus operationStatus, string message, string[] txHashes)
        {
            status.text = $"Signing status: {operationStatus}. Message: {message} ";
            if (operationStatus == OperationStatus.Complete)
            {
                string txs = "";
                foreach (string txHash in txHashes)
                {
                    txs += txHash + "\n";
                }
                status.text = $"Tx pending:\n {txs}";
                transactionsToProcess = txHashes.Length;
                Manager.CheckTransactionStatus(txHashes, TransactionProcessed, 6);
            }
            if (operationStatus == OperationStatus.Error)
            {
                //do something
            }
        }


        /// <summary>
        /// Listener for the transaction status response
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void TransactionProcessed(OperationStatus operationStatus, string tx, string txStatus)
        {
            transactionsToProcess--;
            if (status.text.Contains("Tx pending:"))
            {
                status.text = "";
            }

            status.text += $"Tx: {tx} {operationStatus} {txStatus} \n";

            if (transactionsToProcess == 0)
            {
                //after all transactions are processed, refresh account balance
                if (operationStatus == OperationStatus.Complete)
                {
                    Manager.RefreshAccount(RefreshDone);
                }

            }
        }


        /// <summary>
        /// Callback for the account refresh
        /// </summary>
        /// <param name="operationStatus"></param>
        /// <param name="message"></param>
        private void RefreshDone(OperationStatus operationStatus, string message)
        {
            //status.text = $"Refresh account status: {operationStatus}. Message: {message}";
            if (operationStatus == OperationStatus.Complete)
            {
                //status.text = $"Transaction status: {operationStatus}. Message: {message} -> Refresh tokens";
                //after the account is refreshed load again all tokens.
                //this is not mandatory, you can just load the token that was sent, 
                //or even better just update the token balance in UI with the amount send, without calling the blockchain API
                Manager.LoadAllTokens(TokensLoaded);
            }
        }
    }
}
