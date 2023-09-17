using System;
using System.Numerics;
using System.Threading.Tasks;
using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.Provider.Dtos.Gateway.Addresses;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    /// <summary>
    /// Account object for an Smart Contract
    /// </summary>
    public class AccountSC
    {
        /// <summary>
        /// Account address
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Account EGLD balance
        /// </summary>
        public ESDTAmount Balance { get; private set; }

        /// <summary>
        /// Account nonce
        /// </summary>
        public ulong Nonce { get; private set; }

        /// <summary>
        /// Account shard
        /// </summary>
        public long Shard { get; private set; }

        /// <summary>
        /// Account assets
        /// </summary>
        public dynamic Assets { get; private set; } //JSON data

        /// <summary>
        /// Smart Contract code
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Smart Contract code hash
        /// </summary>
        public string CodeHash { get; private set; }

        /// <summary>
        /// Account root hash
        /// </summary>
        public string RootHash { get; private set; }

        /// <summary>
        /// The number of transactions of Account
        /// </summary>
        public BigInteger TxCount { get; private set; }

        /// <summary>
        /// The number of transactions with smart contracts of Account
        /// </summary>
        public BigInteger SrcCount { get; private set; }

        /// <summary>
        /// Account developer reward
        /// </summary>
        public string DeveloperReward { get; private set; }

        /// <summary>
        /// Smart Contract owner address
        /// </summary>
        public Address OwnerAddress { get; private set; }

        /// <summary>
        /// Smart Contract is upgradable
        /// </summary>
        public bool IsUpgradable { get; private set; }

        /// <summary>
        /// Smart Contract is readable
        /// </summary>
        public bool IsReadable { get; private set; }

        /// <summary>
        /// Smart Contract is payable
        /// </summary>
        public bool IsPayable { get; private set; }

        /// <summary>
        /// Smart Contract is payable by other Smart Contracts
        /// </summary>
        public bool IsPayableBySmartContract { get; private set; }

        /// <summary>
        /// Deploy Transaction Hash
        /// </summary>
        public string DeployTxHash { get; set; }

        /// <summary>
        /// Smart Contract deployed time
        /// </summary>
        public DateTime DeployedAt { get; private set; }

        /// <summary>
        /// Account scam info
        /// </summary>
        public ScamInfo ScamInfo { get; private set; }

        private AccountSC() { }

        /// <summary>
        /// Synchronizes Smart Contract account properties with the ones queried from the API
        /// </summary>
        /// <param name="provider">API provider</param>
        /// <returns></returns>
        public async Task Sync(IApiProvider provider)
        {
            var accountDto = await provider.GetAccount(Address.Bech32);

            Balance = ESDTAmount.From(accountDto.Balance, ESDT.EGLD());
            Nonce = accountDto.Nonce;
            Shard = accountDto.Shard;
            Assets = accountDto.Assets;
            Code = accountDto.Code;
            if (accountDto.CodeHash != null) CodeHash = Converter.ToHexString(Convert.FromBase64String(accountDto.CodeHash)).ToLower();
            if (accountDto.RootHash != null) RootHash = Converter.ToHexString(Convert.FromBase64String(accountDto.RootHash)).ToLower();
            TxCount = accountDto.TxCount;
            SrcCount = accountDto.ScrCount;
            DeveloperReward = accountDto.DeveloperReward;
            OwnerAddress = Address.FromBech32(accountDto.OwnerAddress);
            IsUpgradable = accountDto.IsUpgradable;
            IsReadable = accountDto.IsReadable;
            IsPayable = accountDto.IsPayable;
            IsPayableBySmartContract = accountDto.IsPayableBySmartContract;
            DeployTxHash = accountDto.DeployTxHash;
            DeployedAt = accountDto.DeployedAt.ToDateTime();
            ScamInfo = ScamInfo.From(accountDto.ScamInfo);
        }

        /// <summary>
        /// Synchronizes Smart Contract account properties with the ones queried from the Gateway
        /// </summary>
        /// <param name="provider">Gateway provider</param>
        /// <returns></returns>
        public async Task Sync(IGatewayProvider provider)
        {
            var accountDto = (await provider.GetAddress(Address.Bech32)).Account;

            Address = Address.FromBech32(accountDto.Address);
            Nonce = accountDto.Nonce;
            Balance = ESDTAmount.From(accountDto.Balance, ESDT.EGLD());
            Code = accountDto.Code;
            if (accountDto.CodeHash != null) CodeHash = Converter.ToHexString(Convert.FromBase64String(accountDto.CodeHash)).ToLower();
            if (accountDto.RootHash != null) RootHash = Converter.ToHexString(Convert.FromBase64String(accountDto.RootHash)).ToLower();
            DeveloperReward = accountDto.DeveloperReward;
            OwnerAddress = Address.FromBech32(accountDto.OwnerAddress);
        }

        /// <summary>
        /// Creates a new account object from Gateway data
        /// </summary>
        /// <param name="addressData">Gateway provider</param>
        /// <returns><see cref="AccountSC"/></returns>
        public static AccountSC From(AddressDataDto addressData)
        {
            var account = addressData.Account;

            return new AccountSC()
            {
                Address = Address.FromBech32(account.Address),
                Nonce = account.Nonce,
                Balance = ESDTAmount.From(account.Balance, ESDT.EGLD()),
                Code = account.Code,
                CodeHash = account.CodeHash is null ? null : Converter.ToHexString(Convert.FromBase64String(account.CodeHash)).ToLower(),
                RootHash = account.RootHash is null ? null : Converter.ToHexString(Convert.FromBase64String(account.RootHash)).ToLower(),
                DeveloperReward = account.DeveloperReward,
                OwnerAddress = account.OwnerAddress == "" ? null : Address.FromBech32(account.OwnerAddress)
            };
        }

        /// <summary>
        /// Creates a new Smart Contract account object from API data
        /// </summary>
        /// <param name="account"></param>
        /// <returns><see cref="AccountSC"/></returns>
        public static AccountSC From(Provider.Dtos.API.Accounts.AccountDto account)
        {
            return new AccountSC()
            {
                Address = Address.FromBech32(account.Address),
                Balance = ESDTAmount.From(account.Balance, ESDT.EGLD()),
                Nonce = account.Nonce,
                Shard = account.Shard,
                Assets = account.Assets,
                Code = account.Code,
                CodeHash = account.CodeHash is null ? null : Converter.ToHexString(Convert.FromBase64String(account.CodeHash)).ToLower(),
                RootHash = account.RootHash is null ? null : Converter.ToHexString(Convert.FromBase64String(account.RootHash)).ToLower(),
                TxCount = account.TxCount,
                SrcCount = account.ScrCount,
                DeveloperReward = account.DeveloperReward,
                OwnerAddress = account.OwnerAddress is null ? null : Address.FromBech32(account.OwnerAddress),
                IsUpgradable = account.IsUpgradable,
                IsReadable = account.IsReadable,
                IsPayable = account.IsPayable,
                IsPayableBySmartContract = account.IsPayableBySmartContract,
                DeployTxHash = account.DeployTxHash,
                DeployedAt = account.DeployedAt.ToDateTime(),
                ScamInfo = ScamInfo.From(account.ScamInfo)
            };
        }
    }
}
