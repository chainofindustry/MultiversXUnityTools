using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;
using System.Collections.Generic;

namespace Mx.NET.SDK.TransactionsManager
{
    public class CommonTransactionRequest
    {
        private const string CLAIM_DEVELOPER_REWARDS = "ClaimDeveloperRewards";
        private const string CHANGE_OWNER_ADDRESS = "ChangeOwnerAddress";
        private const string SET_USER_NAME = "SetUserName@";
        private const string SAVE_KEY_VALUE = "SaveKeyValue";

        /// <summary>
        /// Create transaction request - Claim Developer Rewards from Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <returns></returns>
        public static TransactionRequest ClaimDeveloperRewards(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           CLAIM_DEVELOPER_REWARDS);
            transaction.SetGasLimit(new GasLimit(6000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Change Owner Address for an owned Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="newOwner">New Owner address</param>
        /// <returns></returns>
        public static TransactionRequest ChangeOwnerAddress(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            Address newOwner)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           CHANGE_OWNER_ADDRESS,
                                                                                           newOwner);
            transaction.SetGasLimit(new GasLimit(6000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Set User Name for a wallet address
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="username">UserName for account address</param>
        /// <returns></returns>
        public static TransactionRequest SetUserName(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            string username)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           SET_USER_NAME,
                                                                                           BytesValue.FromUtf8(username));
            transaction.SetGasLimit(new GasLimit(1200000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Save data (key-value pairs) under an account storage
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="data">Key-value pairs stored under an account</param>
        /// <returns></returns>
        public static TransactionRequest SaveKeyValue(
            NetworkConfig networkConfig,
            Account account,
            GasLimit gasLimit,
            Dictionary<string, string> data)
        {
            var arguments = new List<IBinaryType>();
            foreach(var pair in data)
            {
                arguments.Add(BytesValue.FromUtf8(pair.Key));
                arguments.Add(BytesValue.FromUtf8(pair.Value));
            }

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           SAVE_KEY_VALUE,
                                                                                           arguments.ToArray());
            transaction.SetGasLimit(gasLimit);

            return transaction;
        }
    }
}
