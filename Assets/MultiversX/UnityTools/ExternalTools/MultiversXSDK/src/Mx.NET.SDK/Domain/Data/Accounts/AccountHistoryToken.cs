using System;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Accounts;
using Mx.NET.SDK.Domain.Helper;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    public class AccountHistoryToken
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

        /// <summary>
        /// Token used at that moment
        /// </summary>
        public string Token { get; set; }

        private AccountHistoryToken() { }

        public static AccountHistoryToken From(AccountHistoryTokenDto accountHistoryToken)
        {
            return new AccountHistoryToken()
            {
                Address = Address.FromBech32(accountHistoryToken.Address),
                Balance = ESDTAmount.From(accountHistoryToken.Balance),
                HistoryTime = accountHistoryToken.Timestamp.ToDateTime(),
                IsSender = accountHistoryToken.IsSender,
                Token = accountHistoryToken.Token
            };
        }
    }
}
