using System;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Accounts;
using Mx.NET.SDK.Domain.Helper;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    public class AccountHistory
    {
        /// <summary>
        /// Account address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Account EGLD Balance
        /// </summary>
        public ESDTAmount Balance { get; set; }

        /// <summary>
        /// History moment
        /// </summary>
        public DateTime HistoryTime { get; set; }

        /// <summary>
        /// Account is sender at that moment
        /// </summary>
        public bool? IsSender { get; set; }

        private AccountHistory() { }

        public static AccountHistory From(AccountHistoryDto accountHistory)
        {
            return new AccountHistory()
            {
                Address = Address.FromBech32(accountHistory.Address),
                Balance = ESDTAmount.From(accountHistory.Balance),
                HistoryTime = accountHistory.Timestamp.ToDateTime(),
                IsSender = accountHistory.IsSender,
            };
        }
    }
}
