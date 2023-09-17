using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.Provider.Dtos.Gateway.Addresses;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    /// <summary>
    /// Account object for an address
    /// </summary>
    public class Account
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
        /// Account user name (herotag)
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Account developer reward
        /// </summary>
        public string DeveloperReward { get; private set; }

        /// <summary>
        /// Account scam info
        /// </summary>
        public ScamInfo ScamInfo { get; private set; }

        /// <summary>
        /// Account is guarded
        /// </summary>
        public bool IsGuarded { get; private set; }

        /// <summary>
        /// Guardian activation epoch
        /// </summary>
        public long ActivationEpoch { get; private set; }

        /// <summary>
        /// Guardian address
        /// </summary>
        public Address Guardian { get; private set; }

        /// <summary>
        /// Guardian Service UID
        /// </summary>
        public string ServiceUID { get; private set; }

        /// <summary>
        /// Pending guardian activation epoch
        /// </summary>
        public long PendingActivationEpoch { get; private set; }

        /// <summary>
        /// Pending guardian address
        /// </summary>
        public Address PendingGuardian { get; private set; }

        /// <summary>
        /// Pending guardian Service UID
        /// </summary>
        public string PendingServiceUID { get; private set; }

        private Account() { }

        public Account(Address address)
        {
            Address = address;
            Nonce = 0;
            Balance = ESDTAmount.Zero();
            UserName = null;
        }

        public Account(string address)
        {
            Address = Address.FromBech32(address);
            Nonce = 0;
            Balance = ESDTAmount.Zero();
            UserName = null;
        }

        /// <summary>
        /// Synchronizes account properties with the ones queried from the API
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
            if (accountDto.RootHash != null) RootHash = Converter.ToHexString(Convert.FromBase64String(accountDto.RootHash)).ToLower();
            TxCount = accountDto.TxCount;
            SrcCount = accountDto.ScrCount;
            UserName = accountDto.UserName;
            DeveloperReward = accountDto.DeveloperReward;
            ScamInfo = ScamInfo.From(accountDto.ScamInfo);
            IsGuarded = accountDto.IsGuarded ?? false;
            ActivationEpoch = accountDto.ActiveGuardianActivationEpoch ?? 0;
            Guardian = accountDto.ActiveGuardianAddress is null ? null : Address.FromBech32(accountDto.ActiveGuardianAddress);
            ServiceUID = accountDto.ActiveGuardianServiceUid ?? string.Empty;
            PendingActivationEpoch = accountDto.PendingGuardianActivationEpoch ?? 0;
            PendingGuardian = accountDto.PendingGuardianAddress is null ? null : Address.FromBech32(accountDto.PendingGuardianAddress);
            PendingServiceUID = accountDto.PendingGuardianServiceUid ?? string.Empty;
        }

        /// <summary>
        /// Synchronizes account + guardian properties with the ones queried from the Gateway
        /// </summary>
        /// <param name="provider">Gateway provider</param>
        /// <returns></returns>
        public async Task SyncWithGuardian(IGatewayProvider provider)
        {
            await Sync(provider);
            await SyncGuardian(provider);
        }

        /// <summary>
        /// Synchronizes account properties with the ones queried from the Gateway
        /// </summary>
        /// <param name="provider">Gateway provider</param>
        /// <returns></returns>
        public async Task Sync(IGatewayProvider provider)
        {
            var accountDto = (await provider.GetAddress(Address.Bech32)).Account;

            Address = Address.FromBech32(accountDto.Address);
            Nonce = accountDto.Nonce;
            Balance = ESDTAmount.From(accountDto.Balance, ESDT.EGLD());
            UserName = accountDto.Username;
            if (accountDto.RootHash != null) RootHash = Converter.ToHexString(Convert.FromBase64String(accountDto.RootHash)).ToLower();
        }

        /// <summary>
        /// Synchronizes account guardian with the ones queried from the Gateway
        /// </summary>
        /// <param name="provider">Gateway provider</param>
        /// <returns></returns>
        public async Task SyncGuardian(IGatewayProvider provider)
        {
            var guardianData = AccountGuardianData.From(await provider.GetAddressGuardianData(Address.Bech32)).GuardianData;

            IsGuarded = guardianData.Guarded ?? false;
            ActivationEpoch = guardianData.ActiveGuardian?.ActivationEpoch ?? 0;
            Guardian = guardianData.ActiveGuardian?.Address;
            ServiceUID = guardianData.ActiveGuardian?.ServiceUID ?? string.Empty;
            PendingActivationEpoch = guardianData.PendingGuardian?.ActivationEpoch ?? 0;
            PendingGuardian = guardianData.PendingGuardian?.Address;
            PendingServiceUID = guardianData.PendingGuardian?.ServiceUID ?? string.Empty;
        }

        /// <summary>
        /// Creates a new account object from Gateway data (No guardian data available - use SyncGuardian method)
        /// </summary>
        /// <param name="addressData"></param>
        /// <returns><see cref="Account"/></returns>
        public static Account From(AddressDataDto addressData)
        {
            var account = addressData.Account;

            return new Account()
            {
                Address = Address.FromBech32(account.Address),
                Nonce = account.Nonce,
                Balance = ESDTAmount.From(account.Balance, ESDT.EGLD()),
                UserName = account.Username,
                RootHash = account.RootHash is null ? null : Converter.ToHexString(Convert.FromBase64String(account.RootHash)).ToLower(),
            };
        }

        /// <summary>
        /// Creates a new account object from API data
        /// </summary>
        /// <param name="account"></param>
        /// <returns><see cref="Account"/></returns>
        public static Account From(Provider.Dtos.API.Accounts.AccountDto account)
        {
            return new Account()
            {
                Address = Address.FromBech32(account.Address),
                Balance = ESDTAmount.From(account.Balance, ESDT.EGLD()),
                Nonce = account.Nonce,
                Shard = account.Shard,
                Assets = account.Assets,
                RootHash = account.RootHash is null ? null : Converter.ToHexString(Convert.FromBase64String(account.RootHash)).ToLower(),
                TxCount = account.TxCount,
                SrcCount = account.ScrCount,
                UserName = account.UserName,
                DeveloperReward = account.DeveloperReward,
                ScamInfo = ScamInfo.From(account.ScamInfo),
                IsGuarded = account.IsGuarded ?? false,
                ActivationEpoch = account.ActiveGuardianActivationEpoch ?? 0,
                Guardian = account.ActiveGuardianAddress is null ? null : Address.FromBech32(account.ActiveGuardianAddress),
                ServiceUID = account.ActiveGuardianServiceUid ?? string.Empty,
                PendingActivationEpoch = account.PendingGuardianActivationEpoch ?? 0,
                PendingGuardian = account.PendingGuardianAddress is null ? null : Address.FromBech32(account.PendingGuardianAddress),
                PendingServiceUID = account.PendingGuardianServiceUid ?? string.Empty
            };
        }

        /// <summary>
        /// Increments (locally) the nonce (Account sequence number).
        /// </summary>
        public void IncrementNonce()
        {
            Nonce++;
        }
    }
}
