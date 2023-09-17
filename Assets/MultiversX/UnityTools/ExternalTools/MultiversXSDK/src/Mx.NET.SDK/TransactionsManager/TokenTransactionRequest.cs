using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Domain.Data.Properties;
using static Mx.NET.SDK.Core.Domain.Constants.Constants;
using static Mx.NET.SDK.Domain.SDKConstants.TokenConstants;
using Mx.NET.SDK.Core.Domain;

namespace Mx.NET.SDK.TransactionsManager
{
    public class TokenTransactionRequest
    {
        private static readonly Regex _nameValidation = new Regex("^[a-zA-Z0-9]{3,20}$");
        private static readonly Regex _tickerValidation = new Regex("^[A-Z0-9]{3,10}$");
        private static readonly Regex _decimalsValidation = new Regex("^([0-9]|1[0-8])$");

        private static readonly Address SYSTEM_SMART_CONTRACT_ADDRESS = Address.FromBech32(ESDT_SMART_CONTRACT);

        private const string ESDT_TRANSFER = "ESDTTransfer";
        private const string ESDT_MULTI_TRANSFER = "MultiESDTNFTTransfer";
        private const string ISSUE = "issue";
        private const string ESDT_LOCAL_MINT = "ESDTLocalMint";
        private const string ESDT_LOCAL_BURN = "ESDTLocalBurn";
        private const string PAUSE = "pause";
        private const string UNPAUSE = "unPause";
        private const string FREEZE = "freeze";
        private const string UNFREEZE = "unFreeze";
        private const string WIPE = "wipe";
        private const string SET_SPECIAL_ROLE = "setSpecialRole";
        private const string UNSET_SPECIAL_ROLE = "unSetSpecialRole";
        private const string SEND_ALL_TRANSFER_ROLES_ADDRESSES = "sendAllTransferRoleAddresses";
        private const string TRANSFER_OWNERSHIP = "transferOwnership";
        private const string CONTROL_CHANGES = "controlChanges";

        /// <summary>
        /// Create transaction request - FungibleESDT Transfer
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="quantity">Nominated quantity (with decimals applied) to transfer</param>
        /// <returns></returns>
        public static TransactionRequest TokenTransfer(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            ESDTIdentifierValue tokenIdentifier,
            ESDTAmount quantity)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           receiver,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_TRANSFER,
                                                                                           tokenIdentifier,
                                                                                           NumericValue.BigUintValue(quantity.Value));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Multiple FungibleESDTs Transfer
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="args">Tuple of ESDTIdentifierValue and ESDTAmount value</param>
        /// <returns></returns>
        public static TransactionRequest MultiTokensTransfer(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            params Tuple<ESDTIdentifierValue, ESDTAmount>[] args)
        {
            var arguments = new List<IBinaryType>
            {
                receiver,
                NumericValue.I32Value(args.Length)
            };
            foreach (var arg in args)
            {
                arguments.Add(arg.Item1);
                arguments.Add(BytesValue.FromHex("00"));
                arguments.Add(NumericValue.BigUintValue(arg.Item2.Value));
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
        /// Create transaction request - FungibleESDT Transfer to Smart Contract without gas limit
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="quantity">Nominated quantity (with decimals applied) to transfer</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest TokenTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            ESDTIdentifierValue tokenIdentifier,
            ESDTAmount quantity,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                tokenIdentifier,
                NumericValue.BigUintValue(quantity.Value),
                BytesValue.FromUtf8(methodName)
            };
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_TRANSFER,
                                                                                           arguments.ToArray());
            //GasLimit: 500000 + extra for SC call
            transaction.SetGasLimit(500000 + GasLimit.FromData(networkConfig, transaction.Data));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Multiple FungibleESDTs Transfer to Smart Contract without gas limit
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="args">Tuple of ESDTIdentifierValue and ESDTAmount value</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest MultiTokensTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            Tuple<ESDTIdentifierValue, ESDTAmount>[] args,
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
                arguments.Add(BytesValue.FromHex("00"));
                arguments.Add(NumericValue.BigUintValue(arg.Item2.Value));
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
        /// Create transaction request - FungibleESDT Transfer to Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="quantity">Nominated quantity (with decimals applied) to transfer</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest TokenTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            ESDTIdentifierValue tokenIdentifier,
            ESDTAmount quantity,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                tokenIdentifier,
                NumericValue.BigUintValue(quantity.Value),
                BytesValue.FromUtf8(methodName)
            };
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_TRANSFER,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(gasLimit);

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Multiple FungibleESDTs Transfer to Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="args">Tuple of ESDTIdentifierValue and ESDTAmount value</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest MultiTokensTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            Tuple<ESDTIdentifierValue, ESDTAmount>[] args,
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
                arguments.Add(BytesValue.FromHex("00"));
                arguments.Add(NumericValue.BigUintValue(arg.Item2.Value));
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
        /// Create transaction request - Issue a FungibleESDT Token with optional properties
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenName">The Token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The Token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <param name="initialSupply">The initial supply</param>
        /// <param name="numberOfDecimals">The number of decimals, should be a numerical value between 0 and 18</param>
        /// <param name="properties">The Token properties</param>
        /// <param name="args">Other arguments for future use</param>
        /// <returns></returns>
        public static TransactionRequest IssueToken(
            NetworkConfig networkConfig,
            Account account,
            string tokenName,
            string tokenTicker,
            ESDTAmount initialSupply,
            int numberOfDecimals,
            TokenProperties properties = null,
            params IBinaryType[] args)
        {
            if (!_nameValidation.IsMatch(tokenName))
                throw new ArgumentException("Length should be between 3 and 20 characters, alphanumeric characters only", nameof(tokenName));

            if (!_tickerValidation.IsMatch(tokenTicker))
                throw new ArgumentException("Length should be between 3 and 10 characters, alphanumeric UPPERCASE characters only", nameof(tokenTicker));

            if (!_decimalsValidation.IsMatch(numberOfDecimals.ToString()))
                throw new ArgumentException("Numerical value should be between 0 and 18", nameof(numberOfDecimals));

            var arguments = new List<IBinaryType>
            {
                BytesValue.FromUtf8(tokenName),
                ESDTIdentifierValue.From(tokenTicker),
                NumericValue.BigUintValue(initialSupply.Value),
                NumericValue.I32Value(numberOfDecimals)
            };

            if (properties != null)
            {
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanFreeze));
                arguments.Add(BytesValue.FromUtf8(properties.CanFreeze.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanWipe));
                arguments.Add(BytesValue.FromUtf8(properties.CanWipe.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanPause));
                arguments.Add(BytesValue.FromUtf8(properties.CanPause.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanMint));
                arguments.Add(BytesValue.FromUtf8(properties.CanMint.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanBurn));
                arguments.Add(BytesValue.FromUtf8(properties.CanBurn.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanChangeOwner));
                arguments.Add(BytesValue.FromUtf8(properties.CanChangeOwner.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanUpgrade));
                arguments.Add(BytesValue.FromUtf8(properties.CanUpgrade.ToString().ToLower()));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanAddSpecialRoles));
                arguments.Add(BytesValue.FromUtf8(properties.CanAddSpecialRoles.ToString().ToLower()));
                arguments.AddRange(args);
            }
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.EGLD("0.05"),
                                                                                           ISSUE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Local Mint operation
        /// 'ESDTRoleLocalMint' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="supplyToMint">The new ESDT token supply to add locally</param>
        /// <returns></returns>
        public static TransactionRequest LocalMint(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            ESDTAmount supplyToMint)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_LOCAL_MINT,
                                                                                           tokenIdentifier,
                                                                                           NumericValue.BigUintValue(supplyToMint.Value));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Local Burn operation
        /// 'ESDTRoleLocalBurn' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="amountToBurn">The ESDT amount to burn locally</param>
        /// <returns></returns>
        public static TransactionRequest LocalBurn(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            ESDTAmount amountToBurn)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_LOCAL_BURN,
                                                                                           tokenIdentifier,
                                                                                           NumericValue.BigUintValue(amountToBurn.Value));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Pause operation (suspend all transactions of the token, except minting, freezing/unfreezing and wiping)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <returns></returns>
        public static TransactionRequest Pause(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           PAUSE,
                                                                                           tokenIdentifier);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Unpause operation (allow transactions with the token again)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <returns></returns>
        public static TransactionRequest Unpause(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNPAUSE,
                                                                                           tokenIdentifier);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Freeze operation (freeze the tokens held by a specific account - no tokens may be transferred to or from the frozen address)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to freeze the Token</param>
        /// <returns></returns>
        public static TransactionRequest Freeze(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           FREEZE,
                                                                                           tokenIdentifier,
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Unfreeze operation (allow transactions with the token to and from the address)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to unfreeze the Token</param>
        /// <returns></returns>
        public static TransactionRequest Unfreeze(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNFREEZE,
                                                                                           tokenIdentifier,
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Wipe operation (wipe out all the tokens held by a frozen address)
        /// Account must be frozen before the wipe operation
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to wipe the Token</param>
        /// <returns></returns>
        public static TransactionRequest Wipe(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           WIPE,
                                                                                           tokenIdentifier,
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Set special role(s) for a given address
        /// 'canAddSpecialRoles' property for token collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token Identifier</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="roles">Roles to assign to receiver address</param>
        /// <returns></returns>
        public static TransactionRequest SetSpecialRole(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            Address receiver,
            params string[] roles)
        {
            var rolesValue = roles.Select(r => (IBinaryType)BytesValue.FromUtf8(r)).ToArray();

            var arguments = new List<IBinaryType>
            {
                tokenIdentifier,
                receiver
            };
            arguments.AddRange(rolesValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
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
        /// 'canAddSpecialRoles' property for token collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="roles">Roles to unassign for receiver address</param>
        /// <returns></returns>
        public static TransactionRequest UnsetSpecialRole(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            Address receiver,
            params string[] roles)
        {
            var rolesValue = roles.Select(r => (IBinaryType)BytesValue.FromUtf8(r)).ToArray();

            var arguments = new List<IBinaryType>
            {
                tokenIdentifier,
                receiver
            };
            arguments.AddRange(rolesValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNSET_SPECIAL_ROLE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Update to follow the latest implementation for token transferability
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <returns></returns>
        public static TransactionRequest SendAllTransferRoleAddresses(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           SEND_ALL_TRANSFER_ROLES_ADDRESSES,
                                                                                           tokenIdentifier);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Transfer management rights to another Account
        /// 'canChangeOwner' property for token collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to receive management rights</param>
        /// <returns></returns>
        public static TransactionRequest TransferOwnership(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           TRANSFER_OWNERSHIP,
                                                                                           tokenIdentifier,
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - ESDT Token properties change
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="properties">Token properties</param>
        /// <param name="args">Other args for future use</param>
        /// <returns></returns>
        public static TransactionRequest ChangeProperties(
            NetworkConfig networkConfig,
            Account account,
            ESDTIdentifierValue tokenIdentifier,
            TokenProperties properties,
            params IBinaryType[] args)
        {
            var arguments = new List<IBinaryType>
            {
                tokenIdentifier,
                BytesValue.FromUtf8(ESDTTokenProperties.CanFreeze),
                BytesValue.FromUtf8(properties.CanFreeze.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTTokenProperties.CanWipe),
                BytesValue.FromUtf8(properties.CanWipe.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTTokenProperties.CanPause),
                BytesValue.FromUtf8(properties.CanPause.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTTokenProperties.CanMint),
                BytesValue.FromUtf8(properties.CanMint.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTTokenProperties.CanBurn),
                BytesValue.FromUtf8(properties.CanBurn.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTTokenProperties.CanChangeOwner),
                BytesValue.FromUtf8(properties.CanChangeOwner.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTTokenProperties.CanUpgrade),
                BytesValue.FromUtf8(properties.CanUpgrade.ToString().ToLower()),
                BytesValue.FromUtf8(ESDTTokenProperties.CanAddSpecialRoles),
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
