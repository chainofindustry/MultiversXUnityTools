using System;
using System.Linq;
using Mx.NET.SDK.Core.Domain.Codec;
using Mx.NET.SDK.Domain.Data.Account;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.SmartContracts;
using static Mx.NET.SDK.Core.Domain.Constants.Constants;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using System.Globalization;

namespace Mx.NET.SDK.Domain
{
    public class TransactionRequest
    {
        private static readonly BinaryCodec binaryCoder = new BinaryCodec();
        private readonly NetworkConfig networkConfig;

        public readonly string ChainId;
        public readonly int Version = 1;
        public Account Account { get; }
        public Address Sender { get; }
        public ulong Nonce { get; }
        public long GasPrice { get; }
        public ESDTAmount Value { get; private set; }
        public Address Receiver { get; private set; }
        public GasLimit GasLimit { get; private set; }
        public string Data { get; private set; }
        public int? Options { get; private set; }

        private TransactionRequest(Account account, NetworkConfig networkConfig)
        {
            this.networkConfig = networkConfig;
            ChainId = networkConfig.ChainId;
            Account = account;
            Sender = account.Address;
            Receiver = Address.Zero();
            Value = ESDTAmount.Zero();
            Nonce = account.Nonce;
            GasLimit = new GasLimit(networkConfig.MinGasLimit);
            GasPrice = networkConfig.MinGasPrice;
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig)
        {
            return new TransactionRequest(account, networkConfig);
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig, Address receiver, ESDTAmount value)
        {
            return new TransactionRequest(account, networkConfig) { Receiver = receiver, Value = value };
        }

        public static TransactionRequest CreateEgldTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address address,
            ESDTAmount value,
            string message = null)
        {
            var transaction = Create(account, networkConfig, address, value);
            transaction.Data = DataCoder.EncodeData(message);
            transaction.SetGasLimit(GasLimit.ForEGLDTransaction(networkConfig, transaction));
            return transaction;
        }

        public static TransactionRequest CreateDeploySmartContractTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            CodeArtifact codeArtifact,
            CodeMetadata codeMetadata,
            params IBinaryType[] args)
        {
            var transaction = Create(account, networkConfig);
            var data = $"{codeArtifact.Value}@{ArwenVirtualMachine}@{codeMetadata.Value}";
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.Data = DataCoder.EncodeData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(networkConfig, transaction));
            return transaction;
        }

        public static TransactionRequest CreateCallSmartContractTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address address,
            ESDTAmount value,
            string methodName,
            params IBinaryType[] args)
        {
            var transaction = Create(account, networkConfig, address, value);
            var data = $"{methodName}";
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.Data = DataCoder.EncodeData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(networkConfig, transaction));
            return transaction;
        }

        public void SetGasLimit(GasLimit gasLimit)
        {
            GasLimit = gasLimit;
        }

        public void SetOptions(int value)
        {
            Options = value;
        }

        public ESDTAmount GetEstimatedFee()
        {
            if (GasLimit is null)
                throw new GasLimitException.UndefinedGasLimitException();

            var dataBytes = Data is null ? Array.Empty<byte>() : Convert.FromBase64String(Data);

            var dataGas = networkConfig.MinGasLimit + dataBytes.Length * networkConfig.GasPerDataByte;
            if (dataGas > GasLimit.Value)
                throw new GasLimitException.NotEnoughGasException($"Not Enough Gas ({GasLimit.Value}) for transaction");

            var gasPrice = networkConfig.MinGasPrice;
            var transactionGas = dataGas * gasPrice;
            if (dataGas == GasLimit.Value)
                return ESDTAmount.From(transactionGas);

            var remainingGas = GasLimit.Value - dataGas;
            var gasPriceModifier = networkConfig.GasPriceModifier;
            var modifiedGasPrice = gasPrice * double.Parse(gasPriceModifier, CultureInfo.InvariantCulture);
            var surplusFee = remainingGas * modifiedGasPrice;

            return ESDTAmount.From($"{transactionGas + surplusFee}");
        }

        public void AddArgument(IBinaryType[] args)
        {
            if (!args.Any())
                return;

            var binaryCodec = new BinaryCodec();
            var decodedData = DataCoder.DecodeData(Data);
            var data = args.Aggregate(decodedData,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCodec.EncodeTopLevel(arg))}");
            Data = DataCoder.EncodeData(data);
        }

        public TransactionRequestDto GetTransactionRequest()
        {
            return new TransactionRequestDto()
            {
                ChainID = ChainId,
                Data = Data,
                GasLimit = GasLimit.Value,
                GasPrice = GasPrice,
                Nonce = Nonce,
                Receiver = Receiver.Bech32,
                Sender = Sender.Bech32,
                Signature = null,
                Value = Value.ToString(),
                Version = Version
            };
        }
    }
}
