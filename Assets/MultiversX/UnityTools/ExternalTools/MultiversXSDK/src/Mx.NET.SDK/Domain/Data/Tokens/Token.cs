using System.Linq;
using System.Numerics;
using Mx.NET.SDK.Domain.Data.Properties;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Tokens;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain.Data.Common;

namespace Mx.NET.SDK.Domain.Data.Tokens
{
    public class Token
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
        /// The number of tokens minted
        /// </summary>
        public string Minted { get; private set; }

        /// <summary>
        /// The number of wiped tokens
        /// </summary>
        public string Burnt { get; private set; }

        /// <summary>
        /// The number of tokens initialy minted
        /// </summary>
        public ESDTAmount InitialMinted { get; private set; }

        /// <summary>
        /// Token decimal precision
        /// </summary>
        public int Decimals { get; private set; }

        /// <summary>
        /// Token is paused
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Token Assets
        /// </summary>
        public dynamic Assets { get; private set; }

        /// <summary>
        /// The number of transactions for Token
        /// </summary>
        public BigInteger Transactions { get; private set; }

        /// <summary>
        /// The number of accounts that possess some tokens
        /// </summary>
        public BigInteger Accounts { get; private set; }

        /// <summary>
        /// Token properties
        /// </summary>
        public TokenProperties Properties { get; private set; }

        /// <summary>
        /// Token price
        /// </summary>
        public string Price { get; private set; }

        /// <summary>
        /// Token market cap
        /// </summary>
        public string MarketCap { get; private set; }

        /// <summary>
        /// Token supply
        /// </summary>
        public string Supply { get; private set; }

        /// <summary>
        /// Token circulating supply
        /// </summary>
        public string CirculatingSupply { get; private set; }

        public TokenRoles[] Roles { get; private set; }

        private Token() { }

        /// <summary>
        /// Creates a new Token from data
        /// </summary>
        /// <param name="token">Token Data Object from API</param>
        /// <returns>Token object</returns>
        public static Token From(TokenDto token)
        {
            return new Token()
            {
                Type = token.Type,
                Identifier = ESDTIdentifierValue.From(token.Identifier),
                Name = token.Name,
                Ticker = token.Ticker ?? token.Identifier.GetTicker(),
                Owner = Address.FromBech32(token.Owner),
                Minted = token.Minted,
                Burnt = token.Burnt,
                InitialMinted = token.InitialMinted is null ? null : ESDTAmount.From(token.InitialMinted,
                                                                                      ESDT.TOKEN(token.Name, token.Identifier, token.Decimals)),
                Decimals = token.Decimals,
                IsPaused = token.IsPaused,
                Assets = token.Assets,
                Accounts = token.Accounts != BigInteger.MinusOne ? token.Accounts : BigInteger.MinusOne,
                Transactions = token.Transactions != BigInteger.MinusOne ? token.Transactions : BigInteger.MinusOne,
                Properties = TokenProperties.From(token.CanFreeze,
                                                  token.CanWipe,
                                                  token.CanPause,
                                                  token.CanMint,
                                                  token.CanBurn,
                                                  token.CanUpgrade,
                                                  token.CanChangeOwner,
                                                  token.CanAddSpecialRoles),
                Price = token.Price,
                MarketCap = token.MarketCap,
                Supply = token.Supply,
                CirculatingSupply = token.CirculatingSupply,
                Roles = TokenRoles.From(token.Roles)
            };
        }

        /// <summary>
        /// Creates a new array of Tokens from data
        /// </summary>
        /// <param name="tokens">Array of Token Data Objects from API</param>
        /// <returns>Array of Token objects</returns>
        public static Token[] From(TokenDto[] tokens)
        {
            return tokens.Select(token => new Token()
            {
                Type = token.Type,
                Identifier = ESDTIdentifierValue.From(token.Identifier),
                Name = token.Name,
                Ticker = token.Ticker ?? token.Identifier.GetTicker(),
                Owner = Address.FromBech32(token.Owner),
                Minted = token.Minted,
                Burnt = token.Burnt,
                InitialMinted = token.InitialMinted is null ? null : ESDTAmount.From(token.InitialMinted,
                                                                                      ESDT.TOKEN(token.Name, token.Identifier, token.Decimals)),
                Decimals = token.Decimals,
                IsPaused = token.IsPaused,
                Assets = token.Assets,
                Accounts = token.Accounts != BigInteger.MinusOne ? token.Accounts : BigInteger.MinusOne,
                Transactions = token.Transactions != BigInteger.MinusOne ? token.Transactions : BigInteger.MinusOne,
                Properties = TokenProperties.From(token.CanFreeze,
                                                  token.CanWipe,
                                                  token.CanPause,
                                                  token.CanMint,
                                                  token.CanBurn,
                                                  token.CanUpgrade,
                                                  token.CanChangeOwner,
                                                  token.CanAddSpecialRoles),
                Price = token.Price,
                MarketCap = token.MarketCap,
                Supply = token.Supply,
                CirculatingSupply = token.CirculatingSupply,
                Roles = TokenRoles.From(token.Roles)
            }).ToArray();
        }

        /// <summary>
        /// Get ESDT object from Token
        /// </summary>
        /// <returns></returns>
        public ESDT GetESDT()
        {
            return ESDT.TOKEN(Name, Identifier.Value, Decimals);
        }
    }
}
