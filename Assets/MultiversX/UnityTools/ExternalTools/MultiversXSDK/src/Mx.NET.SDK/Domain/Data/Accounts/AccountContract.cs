using System;
using System.Linq;
using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Accounts;
using Mx.NET.SDK.Domain.Helper;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    public class AccountContract
    {
        /// <summary>
        /// Contract address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Contract deploy transactions jash
        /// </summary>
        public string DeployTxHash { get; set; }

        /// <summary>
        /// Contract deploy date
        /// </summary>
        public DateTime DeployDate { get; set; }

        /// <summary>
        /// Contract assets
        /// </summary>
        public Assets Assets { get; set; }

        private AccountContract() { }

        /// <summary>
        /// Creates a new array of AccountContract objects from data
        /// </summary>
        /// <param name="accountContracts">Array of AccountContract Data Objects from API</param>
        /// <returns>Array of AccountContract objects</returns>
        public static AccountContract[] From(AccountContractDto[] accountContracts)
        {
            return accountContracts.Select(accountContract => new AccountContract()
            {
                Address = Address.FromBech32(accountContract.Address),
                DeployTxHash = accountContract.DeployTxHash,
                DeployDate = accountContract.Timestamp.ToDateTime(),
                Assets = Assets.From(accountContract.Assets)
            }).ToArray();
        }
    }
}
