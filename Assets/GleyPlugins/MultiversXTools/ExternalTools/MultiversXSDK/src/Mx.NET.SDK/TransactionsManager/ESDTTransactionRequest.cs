using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Account;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Data.Properties;
using Mx.NET.SDK.Core.Domain;
using static Mx.NET.SDK.Core.Domain.Constants.Constants;
using static Mx.NET.SDK.Domain.SDKConstants.ESDTConstants;

namespace Mx.NET.SDK.TransactionsManager
{
    public class ESDTTransactionRequest
    {
        private static readonly Regex _nameValidation = new Regex("^[a-zA-Z0-9]{3,20}$");
        private static readonly Regex _tickerValidation = new Regex("^[A-Z0-9]{3,10}$");
        private static readonly Regex _decimalsValidation = new Regex("^([0-9]|1[0-8])$");

        private static readonly Address SYSTEM_SMART_CONTRACT_ADDRESS = Address.FromBech32(ESDT_SMART_CONTRACT);

        private const string ESDT_NFT_TRANSFER = "ESDTNFTTransfer";
        private const string ESDT_MULTI_TRANSFER = "MultiESDTNFTTransfer";
        private const string ISSUE_NON_FUNGIBLE = "issueNonFungible";
        private const string ISSUE_SEMI_FUNGIBLE = "issueSemiFungible";
        private const string REGISTER_META_ESDT = "registerMetaESDT";
        private const string CHANGE_SFT_INTO_META_ESDT = "changeSFTToMetaESDT";
        private const string ESDT_NFT_CREATE = "ESDTNFTCreate";
        private const string TRANSFER_NFT_CREATE_ROLE = "transferNFTCreateRole";
        private const string STOP_NFT_CREATE = "stopNFTCreate";
        private const string ESDT_NFT_UPDATE_ATTRIBUTES = "ESDTNFTUpdateAttributes";
        private const string ESDT_NFT_ADD_URI = "ESDTNFTAddURI";
        private const string ESDT_NFT_ADD_QUANTITY = "ESDTNFTAddQuantity";
        private const string ESDT_NFT_BURN = "ESDTNFTBurn";
        private const string FREEZE_SINGLE_NFT = "freezeSingleNFT";
        private const string UNFREEZE_SINGLE_NFT = "unFreezeSingleNFT";
        private const string WIPE_SINGLE_NFT = "wipeSingleNFT";
        private const string SET_SPECIAL_ROLE = "setSpecialRole";
        private const string UNSET_SPECIAL_ROLE = "unSetSpecialRole";
        private const string TRANSFER_OWNERSHIP = "transferOwnership";
        private const string CONTROL_CHANGES = "controlChanges";

        /// <summary>
        /// Create transaction request - NFT/SFT/MetaESDT Transfer
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="quantity">Quantity to transfer (1 for NFT transfer)</param>
        /// <returns></returns>
        public static TransactionRequest NFTTransfer(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            ESDTAmount quantity)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_TRANSFER,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.U64Value(nftNonce),
                                                                                           NumericValue.BigUintValue(quantity.Value),
                                                                                           receiver);

            //GasLimit: 1000000 + length of Data field in bytes * erd_gas_per_data_byte
            transaction.SetGasLimit(1000000 + GasLimit.FromData(networkConfig, transaction.Data));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Multiple NFTs/SFTs/MetaESDTs Transfer
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="args">Tuple of ESDTIdentifierValue, nonce and amount</param>
        /// <returns></returns>
        public static TransactionRequest MultiNFTTransfer(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            params Tuple<ESDTIdentifierValue, ulong, ESDTAmount>[] args)
        {
            var arguments = new List<IBinaryType>
            {
                receiver,
                NumericValue.I32Value(args.Length)
            };
            foreach (var arg in args)
            {
                arguments.Add(arg.Item1);
                arguments.Add(NumericValue.U64Value(arg.Item2));
                arguments.Add(NumericValue.BigUintValue(arg.Item3.Value));
            }

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_MULTI_TRANSFER,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(1100000 * args.Length));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - NFT/SFT/MetaESDT Transfer to a Smart Contract without gas limit
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="quantity">Quantity to transfer (1 for NFT transfer)</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract arguments</param>
        /// <returns></returns>
        public static TransactionRequest NFTTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            ESDTAmount quantity,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                NumericValue.U64Value(nftNonce),
                NumericValue.BigUintValue(quantity.Value),
                smartContract,
                BytesValue.FromUtf8(methodName)
            };
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_TRANSFER,
                                                                                           arguments.ToArray());

            //GasLimit: 1000000 + Extra for SC call
            transaction.SetGasLimit(1000000 + GasLimit.FromData(networkConfig, transaction.Data));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Multiple NFT/SFT/MetaESDT Transfer to a Smart Contract without gas limit
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="args">Tuple of ESDTIdentifierValue, nonce and amount</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest MultiNFTTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            Tuple<ESDTIdentifierValue, ulong, ESDTAmount>[] args,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                smartContract,
                NumericValue.I32Value(args.Length)
            };
            foreach (var arg in args)
            {
                arguments.Add(arg.Item1);
                arguments.Add(NumericValue.U64Value(arg.Item2));
                arguments.Add(NumericValue.BigUintValue(arg.Item3.Value));
            }
            arguments.Add(BytesValue.FromUtf8(methodName));
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_MULTI_TRANSFER,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(1100000 * args.Length));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - NFT/SFT/MetaESDT Transfer to a Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="quantity">Quantity to transfer (1 for NFT transfer)</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract arguments</param>
        /// <returns></returns>
        public static TransactionRequest NFTTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            ESDTAmount quantity,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                NumericValue.U64Value(nftNonce),
                NumericValue.BigUintValue(quantity.Value),
                smartContract,
                BytesValue.FromUtf8(methodName)
            };
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_TRANSFER,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(gasLimit);

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Multiple NFT/SFT/MetaESDT Transfer to a Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="args">Tuple of ESDTIdentifierValue, nonce and amount</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest MultiNFTTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            Tuple<ESDTIdentifierValue, ulong, ESDTAmount>[] args,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                smartContract,
                NumericValue.I32Value(args.Length)
            };
            foreach (var arg in args)
            {
                arguments.Add(arg.Item1);
                arguments.Add(NumericValue.U64Value(arg.Item2));
                arguments.Add(NumericValue.BigUintValue(arg.Item3.Value));
            }
            arguments.Add(BytesValue.FromUtf8(methodName));
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_MULTI_TRANSFER,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(gasLimit);

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Issue a Non-Fungible Token with optional properties
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="collectionTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <param name="properties">The Collection properties</param>
        /// <param name="args">Other args for future use</param>
        /// <returns></returns>
        public static TransactionRequest IssueNFT(
            NetworkConfig networkConfig,
            Account account,
            string collectionName,
            string collectionTicker,
            CollectionProperties properties = null,
            params IBinaryType[] args)
        {
            if (!_nameValidation.IsMatch(collectionName))
                throw new ArgumentException("Length should be between 3 and 20 characters, alphanumeric characters only", nameof(collectionName));

            if (!_tickerValidation.IsMatch(collectionTicker))
                throw new ArgumentException("Length should be between 3 and 10 characters, alphanumeric UPPERCASE characters only", nameof(collectionTicker));

            var arguments = new List<IBinaryType>
            {
                BytesValue.FromUtf8(collectionName),
                ESDTIdentifierValue.From(collectionTicker),
            };
            if (properties != null)
            {
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanFreeze));
                arguments.Add(BytesValue.FromUtf8(properties.CanFreeze.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanWipe));
                arguments.Add(BytesValue.FromUtf8(properties.CanWipe.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanPause));
                arguments.Add(BytesValue.FromUtf8(properties.CanPause.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanTransferNftCreateRole));
                arguments.Add(BytesValue.FromUtf8(properties.CanTransferNFTCreateRole.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanChangeOwner));
                arguments.Add(BytesValue.FromUtf8(properties.CanChangeOwner.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanUpgrade));
                arguments.Add(BytesValue.FromUtf8(properties.CanUpgrade.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanAddSpecialRoles));
                arguments.Add(BytesValue.FromUtf8(properties.CanAddSpecialRoles.ToString().ToLower()));
                arguments.AddRange(args);
            }

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.EGLD("0.05"),
                                                                                           ISSUE_NON_FUNGIBLE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Issue a Semi-Fungible Token with optional properties
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="collectionTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <param name="properties">The Collection properties</param>
        /// <param name="args">Other args for future use</param>
        /// <returns></returns>
        public static TransactionRequest IssueSFT(
            NetworkConfig networkConfig,
            Account account,
            string collectionName,
            string collectionTicker,
            CollectionProperties properties = null,
            params IBinaryType[] args)
        {
            var cost = networkConfig.ChainId == "T" ? ESDTAmount.EGLD("5") : ESDTAmount.EGLD("0.05");

            if (!_nameValidation.IsMatch(collectionName))
                throw new ArgumentException("Length should be between 3 and 20 characters, alphanumeric characters only", nameof(collectionName));

            if (!_tickerValidation.IsMatch(collectionTicker))
                throw new ArgumentException("Length should be between 3 and 10 characters, alphanumeric UPPERCASE characters only", nameof(collectionTicker));

            var arguments = new List<IBinaryType>
            {
                BytesValue.FromUtf8(collectionName),
                ESDTIdentifierValue.From(collectionTicker)
            };
            if (properties != null)
            {
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanFreeze));
                arguments.Add(BytesValue.FromUtf8(properties.CanFreeze.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanWipe));
                arguments.Add(BytesValue.FromUtf8(properties.CanWipe.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanPause));
                arguments.Add(BytesValue.FromUtf8(properties.CanPause.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanTransferNftCreateRole));
                arguments.Add(BytesValue.FromUtf8(properties.CanTransferNFTCreateRole.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanChangeOwner));
                arguments.Add(BytesValue.FromUtf8(properties.CanChangeOwner.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanUpgrade));
                arguments.Add(BytesValue.FromUtf8(properties.CanUpgrade.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanAddSpecialRoles));
                arguments.Add(BytesValue.FromUtf8(properties.CanAddSpecialRoles.ToString().ToLower()));
                arguments.AddRange(args);
            }

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           cost,
                                                                                           ISSUE_SEMI_FUNGIBLE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Issue a MetaESDT with optional properties
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="collectionTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <param name="numberOfDecimals">Number of decimals, should be a numerical value between 0 and 18</param>
        /// <param name="properties">The Collection properties</param>
        /// <param name="args">Other args for future use</param>
        /// <returns></returns>
        public static TransactionRequest IssueMetaESDT(
            NetworkConfig networkConfig,
            Account account,
            string collectionName,
            string collectionTicker,
            int numberOfDecimals,
            CollectionProperties properties = null,
            params IBinaryType[] args)
        {
            var cost = networkConfig.ChainId == "T" ? ESDTAmount.EGLD("5") : ESDTAmount.EGLD("0.05");

            if (!_nameValidation.IsMatch(collectionName))
                throw new ArgumentException("Length should be between 3 and 20 characters, alphanumeric characters only", nameof(collectionName));

            if (!_tickerValidation.IsMatch(collectionTicker))
                throw new ArgumentException("Length should be between 3 and 10 characters, alphanumeric UPPERCASE characters only", nameof(collectionTicker));

            if (!_decimalsValidation.IsMatch(numberOfDecimals.ToString()))
                throw new ArgumentException("Numerical value should be between 0 and 18", nameof(numberOfDecimals));

            var arguments = new List<IBinaryType>
            {
                BytesValue.FromUtf8(collectionName),
                ESDTIdentifierValue.From(collectionTicker),
                NumericValue.I32Value(numberOfDecimals)
            };
            if (properties != null)
            {
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanFreeze));
                arguments.Add(BytesValue.FromUtf8(properties.CanFreeze.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanWipe));
                arguments.Add(BytesValue.FromUtf8(properties.CanWipe.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanPause));
                arguments.Add(BytesValue.FromUtf8(properties.CanPause.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanTransferNftCreateRole));
                arguments.Add(BytesValue.FromUtf8(properties.CanTransferNFTCreateRole.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanChangeOwner));
                arguments.Add(BytesValue.FromUtf8(properties.CanChangeOwner.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanUpgrade));
                arguments.Add(BytesValue.FromUtf8(properties.CanUpgrade.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTCollectionProperties.CanAddSpecialRoles));
                arguments.Add(BytesValue.FromUtf8(properties.CanAddSpecialRoles.ToString().ToLower()));
                arguments.AddRange(args);
            }

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           cost,
                                                                                           REGISTER_META_ESDT,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Convert SFT into MetaESDT
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="numberOfDecimals">Number of decimals, should be a numerical value between 0 and 18</param>
        /// <returns></returns>
        public static TransactionRequest ConvertSFTIntoMetaEESDT(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            int numberOfDecimals)
        {
            if (!_decimalsValidation.IsMatch(numberOfDecimals.ToString()))
                throw new ArgumentException("Numerical value should be between 0 and 18", nameof(numberOfDecimals));

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           CHANGE_SFT_INTO_META_ESDT,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.I32Value(numberOfDecimals));

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Create NFT/SFT
        /// 'ESDTRoleNFTCreate' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Account with ESDTRoleNFTCreate role</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="quantity">The quantity of NFT/SFT (1 for NFT)</param>
        /// <param name="name">The name of the NFT/SFT</param>
        /// <param name="royalties">Allows the creator to receive royalties for any transaction involving their NFT/SFT (Base format is a numeric value between 0 an 10000 (0 meaning 0% and 10000 meaning 100%)</param>
        /// <param name="hash">Arbitrary field that should contain the hash of the NFT/SFT properties or null</param>
        /// <param name="attributes">Arbitrary field that should contain a set of attributes in the format desired by the creator</param>
        /// <param name="uris">Minimum one field that should contain the Uniform Resource Identifier. Can be a URL to a media file or something similar.</param>
        /// <returns></returns>
        public static TransactionRequest CreateNFT(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            BigInteger quantity,
            string name,
            ushort royalties,
            byte[] hash,
            Dictionary<string, string> attributes,
            Uri[] uris)
        {
            if (royalties > 10000)
                throw new ArgumentException("Value should be between 0 and 10000 (0 meaning 0% and 10000 meaning 100%",
                                            nameof(royalties));

            if (uris.Length == 0)
                throw new ArgumentException("At least one URI should be provided", nameof(uris));


            var attributeValue = string.Join(";", attributes.Select(x => x.Key + ":" + x.Value).ToArray());
            var urisValue = uris.Select(u => (IBinaryType)BytesValue.FromUtf8(u.AbsoluteUri)).ToArray();

            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                NumericValue.BigUintValue(quantity),
                ESDTIdentifierValue.From(name),
                NumericValue.U16Value(royalties),
                BytesValue.FromBuffer(hash ?? Array.Empty<byte>()),
                BytesValue.FromUtf8(attributeValue)
            };
            arguments.AddRange(urisValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_CREATE,
                                                                                           arguments.ToArray());


            const int storePerByte = 50000;
            // Storage cost: (Size of NFT attributes + URIs) * 50000 (storePerByte)
            var storageCost = (string.IsNullOrEmpty(attributeValue) ? 0
                                  : storePerByte * BytesValue.FromUtf8(attributeValue).GetLength()) +
                                    storePerByte * urisValue.Sum(u => u.ValueOf<BytesValue>().GetLength());

            transaction.SetGasLimit(3000000 + storageCost + GasLimit.FromData(networkConfig, transaction.Data));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Create MetaESDT
        /// 'ESDTRoleNFTCreate' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Account with ESDTRoleNFTCreate role</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="quantity">The quantity of MetaESDT</param>
        /// <param name="name">The name of the MetaESDT</param>
        /// <returns></returns>
        public static TransactionRequest CreateMetaESDT(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            BigInteger quantity,
            string name)
        {
            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                NumericValue.BigUintValue(quantity),
                ESDTIdentifierValue.From(name),
                BytesValue.FromUtf8("0"), //royalties
                BytesValue.FromUtf8("0"), //hash
                BytesValue.FromUtf8("0"), //attributes
                BytesValue.FromUtf8("0") //uris
            };

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_CREATE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(3000000 + GasLimit.FromData(networkConfig, transaction.Data));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Transfer NFT/SFT/MetaESDT create role
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="fromAddress">Address to transfer the role from</param>
        /// <param name="toAddress">Address to transfer to role to</param>
        /// <returns></returns>
        public static TransactionRequest TransferNFTCreateRole(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            Address fromAddress,
            Address toAddress)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           TRANSFER_NFT_CREATE_ROLE,
                                                                                           collectionIdentifier,
                                                                                           fromAddress,
                                                                                           toAddress);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Stop NFT/SFT/MetaESDT create for a given collection
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <returns></returns>
        public static TransactionRequest StopNFTCreate(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           STOP_NFT_CREATE,
                                                                                           collectionIdentifier);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - NFT (only) Update Attributes
        /// 'ESDTRoleNFTUpdateAttributes' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="attributes">Arbitrary field that should contain a set of attributes in the format desired by the creator</param>
        /// <returns></returns>
        public static TransactionRequest NFTUpdateAttributes(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            Dictionary<string, string> attributes)
        {
            var attributeValue = string.Join(";", attributes.Select(x => x.Key + ":" + x.Value).ToArray());

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_UPDATE_ATTRIBUTES,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.U64Value(nftNonce),
                                                                                           BytesValue.FromUtf8(attributeValue));

            transaction.SetGasLimit(new GasLimit(1000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - NFT (only) Add URIs
        /// 'ESDTRoleNFTAddURI' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="uris">Minimum one field that should contain the Uniform Resource Identifier. Can be a URL to a media file or something similar.</param>
        /// <returns></returns>
        public static TransactionRequest NFTAddUri(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            Uri[] uris)
        {
            var urisValue = uris.Select(u => (IBinaryType)BytesValue.FromUtf8(u.AbsoluteUri)).ToArray();

            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                NumericValue.U64Value(nftNonce)
            };
            arguments.AddRange(urisValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_ADD_URI,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(1000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - SFT (only) Add quantity
        /// 'ESDTRoleNFTAddQuantity' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="sftNonce">SFT nonce</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns></returns>
        public static TransactionRequest SFTAddQuantity(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            ulong sftNonce,
            BigInteger quantity)
        {

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_ADD_QUANTITY,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.U64Value(sftNonce),
                                                                                           NumericValue.BigUintValue(quantity));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - NFT/SFT/MetaESDT burn quantity
        /// 'ESDTRoleNFTBurn' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="quantity">Quantity to burn</param>
        /// <returns></returns>
        public static TransactionRequest NFTBurn(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            BigInteger quantity)
        {

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_NFT_BURN,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.U64Value(nftNonce),
                                                                                           NumericValue.BigUintValue(quantity));

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request -  Freeze single NFT/SFT
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="receiver">Address to freeze the NFT/SFT</param>
        /// <returns></returns>
        public static TransactionRequest FreezeSingleNFT(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           FREEZE_SINGLE_NFT,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.U64Value(nftNonce),
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Unfreeze single NFT/SFT
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="receiver">Address to unfreeze the NFT/SFT</param>
        /// <returns></returns>
        public static TransactionRequest UnfreezeSingleNFT(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNFREEZE_SINGLE_NFT,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.U64Value(nftNonce),
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Wipe single NFT/SFTs
        /// Account must be frozen before the wipe operation
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="nftNonce">NFT nonce</param>
        /// <param name="receiver">Address to wipe the NFT/SFT</param>
        /// <returns></returns>
        public static TransactionRequest WipeSingleNFT(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            ulong nftNonce,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           WIPE_SINGLE_NFT,
                                                                                           collectionIdentifier,
                                                                                           NumericValue.U64Value(nftNonce),
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Set special role(s) for a given address
        /// 'canAddSpecialRoles' property for NFT/SFT/MetaESDT collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="collectionIdentifier">Collection Identifier</param>
        /// <param name="roles">Roles to assign to receiver address</param>
        /// <returns></returns>
        public static TransactionRequest SetSpecialRole(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            Address receiver,
            params string[] roles)
        {
            var rolesValue = roles.Select(r => (IBinaryType)BytesValue.FromUtf8(r)).ToArray();

            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                receiver
            };
            arguments.AddRange(rolesValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                           networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           SET_SPECIAL_ROLE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Unset special role(s) for a given address
        /// 'canAddSpecialRoles' property for NFT/SFT/MetaESDT collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="collectionIdentifier">Collection Identifier</param>
        /// <param name="roles">Roles to unassign for receiver address</param>
        /// <returns></returns>
        public static TransactionRequest UnsetSpecialRole(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            Address receiver,
            params string[] roles)
        {
            var rolesValue = roles.Select(r => (IBinaryType)BytesValue.FromUtf8(r)).ToArray();

            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                receiver
            };
            arguments.AddRange(rolesValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                           networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNSET_SPECIAL_ROLE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Transfer management rights to another Account
        /// 'canChangeOwner' property for NFT/SFT/MetaESDT collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="receiver">Address to receive management rights</param>
        /// <returns></returns>
        public static TransactionRequest TransferOwnership(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           TRANSFER_OWNERSHIP,
                                                                                           collectionIdentifier,
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - NFT collection properties change
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <param name="properties">Nft properties object</param>
        /// <param name="args">Other args for future use</param>
        /// <returns></returns>
        public static TransactionRequest ChangeProperties(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue collectionIdentifier,
            CollectionProperties properties,
            params IBinaryType[] args)
        {
            var arguments = new List<IBinaryType>
            {
                collectionIdentifier,
                BytesValue.FromUtf8(ESDTCollectionProperties.CanFreeze),
                BytesValue.FromUtf8(properties.CanFreeze.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTCollectionProperties.CanWipe),
                BytesValue.FromUtf8(properties.CanWipe.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTCollectionProperties.CanPause),
                BytesValue.FromUtf8(properties.CanPause.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTCollectionProperties.CanTransferNftCreateRole),
                BytesValue.FromUtf8(properties.CanTransferNFTCreateRole.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTCollectionProperties.CanChangeOwner),
                BytesValue.FromUtf8(properties.CanChangeOwner.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTCollectionProperties.CanUpgrade),
                BytesValue.FromUtf8(properties.CanUpgrade.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTCollectionProperties.CanAddSpecialRoles),
                BytesValue.FromUtf8(properties.CanAddSpecialRoles.ToString().ToLower())
            };
            arguments.AddRange(args);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           CONTROL_CHANGES,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }
    }
}
