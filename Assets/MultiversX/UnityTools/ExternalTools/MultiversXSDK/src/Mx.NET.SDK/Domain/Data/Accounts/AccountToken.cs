using System.Linq;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Accounts;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    /// <summary>
    /// Account Token object
    /// </summary>
    public class AccountToken
    {
        /// <summary>
        /// Token Type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Token Identifier
        /// </summary>
        public ESDTIdentifierValue Identifier { get; private set; }

        /// <summary>
        /// Token name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Token ticker
        /// </summary>
        public string Ticker { get; private set; }

        /// <summary>
        /// Token owner
        /// </summary>
        public Address Owner { get; private set; }

        /// <summary>
        /// Token decimal precision
        /// </summary>
        public int Decimals { get; private set; }

        /// <summary>
        /// Token is paused
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Token assets
        /// </summary>
        public dynamic Assets { get; private set; }

        /// <summary>
        /// Token price
        /// </summary>
        public string Price { get; private set; }

        /// <summary>
        /// Token balance
        /// </summary>
        public ESDTAmount Balance { get; private set; }

        /// <summary>
        /// Token USD value of the balance owned
        /// </summary>
        public string ValueUSD { get; private set; }

        private AccountToken() { }

        /// <summary>
        /// Creates a new AccountToken from data
        /// </summary>
        /// <param name="token">Token Data Object from API</param>
        /// <returns>AccountToken object</returns>
        public static AccountToken From(AccountTokenDto token)
        {
            return new AccountToken()
            {
                Type = token.Type,
                Identifier = ESDTIdentifierValue.From(token.Identifier),
                Name = token.Name,
                Ticker = token.Ticker ?? token.Identifier.GetTicker(),
                Owner = Address.FromBech32(token.Owner),
                Decimals = token.Decimals,
                IsPaused = token.IsPaused,
                Assets = token.Assets,
                Price = token.Price,
                Balance = token.Balance is null ? null : ESDTAmount.From(token.Balance,
                                                                          ESDT.TOKEN(token.Name, token.Identifier, token.Decimals)),
                ValueUSD = token.ValueUSD
            };
        }

        /// <summary>
        /// Creates a new array of AccountToken from data
        /// </summary>
        /// <param name="tokens">Array of Token Data Objects from API</param>
        /// <returns>Array of AccountToken objects</returns>
        public static AccountToken[] From(AccountTokenDto[] tokens)
        {
            return tokens.Select(token => new AccountToken()
            {
                Type = token.Type,
                Identifier = ESDTIdentifierValue.From(token.Identifier),
                Name = token.Name,
                Ticker = token.Ticker ?? token.Identifier.GetTicker(),
                Owner = Address.FromBech32(token.Owner),
                Decimals = token.Decimals,
                IsPaused = token.IsPaused,
                Assets = token.Assets,
                Price = token.Price,
                Balance = token.Balance is null ? null : ESDTAmount.From(token.Balance,
                                                                          ESDT.TOKEN(token.Name, token.Identifier, token.Decimals)),
                ValueUSD = token.ValueUSD
            }).ToArray();
        }

        /// <summary>
        /// Get the ESDT object from Token
        /// </summary>
        /// <returns></returns>
        public ESDT GetESDT()
        {
            return ESDT.TOKEN(Name, Identifier.Value, Decimals);
        }
    }
}
