using System;
using System.Linq;
using Mx.NET.SDK.Core.Domain.Codec;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.SmartContracts;
using static Mx.NET.SDK.Core.Domain.Constants.Constants;
using System.Globalization;
using static Mx.NET.SDK.Core.Domain.Values.TypeValue;
using System.Text;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;

namespace Mx.NET.SDK.Domain
{
    public class TransactionRequest
    {
        private static readonly BinaryCodec binaryCoder = new BinaryCodec();
        private readonly NetworkConfig networkConfig;
        public Account Account { get; }

        public ulong Nonce { get; }
        public ESDTAmount Value { get; private set; }
        public Address Receiver { get; private set; }
        public Address Sender { get; }
        public long GasPrice { get; }
        public GasLimit GasLimit { get; private set; }
        public string ChainId { get; }
        public string Data { get; private set; }
        public int Version { get; private set; } = 1;
        public int? Options { get; private set; }
        public Address Guardian { get; private set; } = null;

        private TransactionRequest(Account account, NetworkConfig networkConfig)
        {
            this.networkConfig = networkConfig;
            Account = account;

            Nonce = account.Nonce;
            Value = ESDTAmount.Zero();
            Receiver = Address.Zero();
            Sender = account.Address;
            GasPrice = networkConfig.MinGasPrice;
            GasLimit = new GasLimit(networkConfig.MinGasLimit);
            ChainId = networkConfig.ChainId;
            if (account.IsGuarded)
            {
                Guardian = account.Guardian;
                Version = 2;
                Options = 2;
            }
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig)
        {
            return new TransactionRequest(account, networkConfig);
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig, Address receiver, ESDTAmount value)
        {
            return new TransactionRequest(account, networkConfig) { Receiver = receiver, Value = value };
        }

        public void SetGasLimit(GasLimit gasLimit)
        {
            if (Account.IsGuarded)
                gasLimit += 50000;
            GasLimit = gasLimit;
        }

        public void SetVersion(int value)
        {
            Version = value;
        }

        public void SetOptions(int value)
        {
            Options = value;
        }

        public void SetGuardian(string address)
        {
            Guardian = Address.FromBech32(address);
            Version = 2;
            Options = 2;
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
                                      (c, arg) =>
                                      {
                                          var hex = Converter.ToHexString(binaryCoder.EncodeTopLevel(arg));
                                          //In case of OptionalValue, if there is no value we shouldn't put the parameter.
                                          var hexFormat = arg.Type.BinaryType == BinaryTypes.Optional && string.IsNullOrEmpty(hex) ? string.Empty : $"@{hex}";
                                          return c + hexFormat;
                                      });
            }

            transaction.Data = DataCoder.EncodeData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(networkConfig, transaction));
            return transaction;
        }

        public ESDTAmount GetEstimatedFee()
        {
            if (GasLimit is null)
                throw new GasLimitException.UndefinedGasLimitException();

            var dataBytes = Data is null ? Array.Empty<byte>() : Convert.FromBase64String(Data);

            var dataGas = networkConfig.MinGasLimit + dataBytes.Length * networkConfig.GasPerDataByte;
            if (Account.IsGuarded)
                dataGas += 50000;
            if (dataGas > GasLimit.Value)
                throw new GasLimitException.NotEnoughGasException($"Not Enough Gas ({GasLimit.Value}) for transaction");

            var gasPrice = networkConfig.MinGasPrice;
            var transactionGas = dataGas * gasPrice;
            if (dataGas == GasLimit.Value)
                return ESDTAmount.From($"{transactionGas}");

            var remainingGas = GasLimit.Value - dataGas;
            var gasPriceModifier = networkConfig.GasPriceModifier;
            var modifiedGasPrice = gasPrice * double.Parse(gasPriceModifier, CultureInfo.InvariantCulture);
            var surplusFee = remainingGas * modifiedGasPrice;

            return ESDTAmount.From($"{transactionGas + surplusFee}");
        }

        public TransactionRequestDto GetTransactionRequest()
        {
            return new TransactionRequestDto()
            {
                Nonce = Nonce,
                Value = Value.ToString(),
                Receiver = Receiver.Bech32,
                Sender = Sender.Bech32,
                GasPrice = GasPrice,
                GasLimit = GasLimit.Value,
                Data = Data,
                ChainID = ChainId,
                Version = Version,
                Options = Options,
                Guardian = Guardian?.Bech32,
                Signature = null,
                GuardianSignature = null
            };
        }

        public byte[] SerializeForSigning()
        {
            var transactionRequest = GetTransactionRequest();
            var data = JsonWrapper.Serialize(transactionRequest);
            return Encoding.UTF8.GetBytes(data);
        }

        public TransactionRequestDto ApplySignature(string signature)
        {
            var transactionRequest = GetTransactionRequest();
            transactionRequest.Signature = signature;
            return transactionRequest;
        }

        public TransactionRequestDto ApplyGuardianSignature(string guardianSignature)
        {
            var transactionRequest = GetTransactionRequest();
            transactionRequest.GuardianSignature = guardianSignature;
            return transactionRequest;
        }
    }
}
