using System;
using Mx.NET.SDK.Domain.Data.Network;

namespace Mx.NET.SDK.Domain
{
    public class GasLimit
    {
        public long Value { get; }

        public GasLimit(long value)
        {
            Value = value;
        }

        public static GasLimit operator +(GasLimit gasLimit1, GasLimit gasLimit2) => new GasLimit(gasLimit1.Value + gasLimit2.Value);
        public static GasLimit operator +(long gas, GasLimit gasLimit) => new GasLimit(gasLimit.Value + gas);
        public static GasLimit operator +(GasLimit gasLimit, long gas) => new GasLimit(gasLimit.Value + gas);

        /// <summary>
        /// Compute GasLimit for transaction
        /// </summary>
        /// <param name="networkConfig">The network config</param>
        /// <param name="transaction">The transaction</param>
        /// <returns>A GasLimit</returns>
        public static GasLimit ForEGLDTransaction(NetworkConfig networkConfig, TransactionRequest transaction)
        {
            var value = networkConfig.MinGasLimit;
            return new GasLimit(value) + FromData(networkConfig, transaction.Data);
        }

        /// <summary>
        /// Compute transaction Data GasLimit 
        /// </summary>
        /// <param name="networkConfig">The network config</param>
        /// <param name="data">The transaction data</param>
        /// <returns>A GasLimit</returns>
        public static GasLimit FromData(NetworkConfig networkConfig, string data)
        {
            if (string.IsNullOrEmpty(data))
                return new GasLimit(0);

            var bytes = Convert.FromBase64String(data);
            var value = networkConfig.GasPerDataByte * bytes.Length;

            return new GasLimit(value);
        }

        /// <summary>
        /// Compute GasLimit for a smart contract call
        /// </summary>
        /// <param name="networkConfig">The network config</param>
        /// <param name="transaction">The transaction</param>
        /// <returns>A GasLimit</returns>
        public static GasLimit ForSmartContractCall(NetworkConfig networkConfig, TransactionRequest transaction)
        {
            var value = networkConfig.MinGasLimit + 6000000;
            return new GasLimit(value) + FromData(networkConfig, transaction.Data);
        }
    }
}
