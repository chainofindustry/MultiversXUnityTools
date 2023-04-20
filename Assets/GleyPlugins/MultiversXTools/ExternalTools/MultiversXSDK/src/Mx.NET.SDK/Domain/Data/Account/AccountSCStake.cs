using System;
using System.Linq;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain.Helper;
using Mx.NET.SDK.Provider.Dtos.API.Account;

namespace Mx.NET.SDK.Domain.Data.Account
{
    public class AccountSCStake
    {
        /// <summary>
        /// Total EGLD staked
        /// </summary>
        public ESDTAmount TotalStaked { get; set; }

        /// <summary>
        /// Unbound EGLD tokens
        /// </summary>
        public UnstakedToken[] UnstakedTokens { get; set; }

        private AccountSCStake() { }

        public static AccountSCStake From(AccountSCStakeDto scStake)
        {
            return new AccountSCStake()
            {
                TotalStaked = ESDTAmount.From(scStake.TotalStaked),
                UnstakedTokens = UnstakedToken.From(scStake.UnstakedTokens)
            };
        }
    }

    public class UnstakedToken
    {
        /// <summary>
        /// EGLD amount
        /// </summary>
        public ESDTAmount Amount { get; set; }

        /// <summary>
        /// Time when tokens will be unlocked
        /// </summary>
        public DateTime UnboundPeriod { get; set; }

        private UnstakedToken() { }

        public static UnstakedToken[] From(UnstakedTokenDto[] scUnstake)
        {
            return scUnstake.Select(unstaked => new UnstakedToken()
            {
                Amount = ESDTAmount.From(unstaked.Amount),
                UnboundPeriod = unstaked.Expires is null ? default : ((long)unstaked.Expires).ToDateTime()
            }).ToArray();
        }

        public bool UnboundPeriodExpired()
        {
            return UnboundPeriod == default;
        }
    }
}
