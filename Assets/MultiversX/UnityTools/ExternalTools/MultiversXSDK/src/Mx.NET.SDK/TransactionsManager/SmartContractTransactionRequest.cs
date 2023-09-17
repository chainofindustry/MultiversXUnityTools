using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.SmartContracts;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;

namespace Mx.NET.SDK.TransactionsManager
{
    public class SmartContractTransactionRequest
    {
        private const string UPGRADE_CONTRACT = "upgradeContract";

        /// <summary>
        /// Create transaction request - EGLD Transfer to Smart Contract with 60M gas
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender/Owner Account</param>
        /// <param name="codeArtifact"></param>
        /// <param name="codeMetadata"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TransactionRequest Deploy(
            NetworkConfig networkConfig,
            Account account,
            CodeArtifact codeArtifact,
            CodeMetadata codeMetadata,
            params IBinaryType[] args)
        {
            var transaction = TransactionRequest.CreateDeploySmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           codeArtifact,
                                                                                           codeMetadata,
                                                                                           args);
            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - EGLD Transfer to Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender/Owner Account</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="codeArtifact"></param>
        /// <param name="codeMetadata"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TransactionRequest Deploy(
            NetworkConfig networkConfig,
            Account account,
            GasLimit gasLimit,
            CodeArtifact codeArtifact,
            CodeMetadata codeMetadata,
            params IBinaryType[] args)
        {
            var transaction = TransactionRequest.CreateDeploySmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           codeArtifact,
                                                                                           codeMetadata,
                                                                                           args);
            transaction.SetGasLimit(gasLimit);

            return transaction;
        }

        /// <summary>
        /// Create transaction request - EGLD Transfer to Smart Contract with 60M gas
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender/Owner Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="codeArtifact"></param>
        /// <param name="codeMetadata"></param>
        /// <returns></returns>
        public static TransactionRequest Upgrade(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            CodeArtifact codeArtifact,
            CodeMetadata codeMetadata)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           UPGRADE_CONTRACT,
                                                                                           BytesValue.FromHex(codeArtifact.Value),
                                                                                           BytesValue.FromHex(codeMetadata.Value));
            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - EGLD Transfer to Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender/Owner Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="codeArtifact"></param>
        /// <param name="codeMetadata"></param>
        /// <returns></returns>
        public static TransactionRequest Upgrade(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            CodeArtifact codeArtifact,
            CodeMetadata codeMetadata)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           UPGRADE_CONTRACT,
                                                                                           BytesValue.FromHex(codeArtifact.Value),
                                                                                           BytesValue.FromHex(codeMetadata.Value));
            transaction.SetGasLimit(gasLimit);

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Smart Contract Call (equivalent to EGLDTransferToSmartContract function)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest Call(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            return EGLDTransactionRequest.EGLDTransferToSmartContract(networkConfig,
                                                                      account,
                                                                      smartContract,
                                                                      gasLimit,
                                                                      ESDTAmount.Zero(),
                                                                      methodName,
                                                                      methodArgs);
        }
    }
}
