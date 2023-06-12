using System;
using System.Numerics;
using System.Threading.Tasks;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Provider.API;
using Mx.NET.SDK.Provider.Dtos.API.Account;

namespace Mx.NET.SDK.Domain.Data.Account
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

        private Account() { }

        public Account(Address address)
        {
            Address = address;
            Nonce = 0;
            Balance = ESDTAmount.Zero();
            UserName = null;
        }

        /// <summary>
        /// Synchronizes account properties with the ones queried from the Network
        /// </summary>
        /// <param name="provider"></param>
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
        }

        /// <summary>
        /// Creates a new account object from data
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Account From(AccountDto account)
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
                ScamInfo = ScamInfo.From(account.ScamInfo)
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
