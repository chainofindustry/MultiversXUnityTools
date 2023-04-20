using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Account;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Core.Domain;

namespace Mx.NET.SDK.TransactionsManager
{
    public class EGLDTransactionRequest
    {
        /// <summary>
        /// Create transaction request - EGLD Transfer
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="egldValue">EGLD amount to send</param>
        /// <param name="message">The message for receiver</param>
        /// <returns></returns>
        public static TransactionRequest EGLDTransfer(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            ESDTAmount egldValue,
            string message = null)
        {
            return TransactionRequest.CreateEgldTransactionRequest(networkConfig,
                                                                   account,
                                                                   receiver,
                                                                   egldValue,
                                                                   message);
        }

        /// <summary>
        /// Create transaction request - EGLD Transfer to Smart Contract without gas limit
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="egldValue">EGLD amount to send</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest EGLDTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            ESDTAmount egldValue,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           egldValue,
                                                                                           methodName,
                                                                                           methodArgs);
            //GasLimit: 500000 + extra for SC call
            transaction.SetGasLimit(500000 + GasLimit.FromData(networkConfig, transaction.Data));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - EGLD Transfer to Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="egldValue">EGLD amount to send</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest EGLDTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            ESDTAmount egldValue,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           egldValue,
                                                                                           methodName,
                                                                                           methodArgs);
            transaction.SetGasLimit(gasLimit);

            return transaction;
        }
    }
}
