using Mx.NET.SDK.Provider.Dtos.API.Common;
using Mx.NET.SDK.Provider.Dtos.API.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface ITokensProvider
    {
        /// <summary>
        /// Returns an array of Tokens
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="TokenDto"/></returns>
        Task<TokenDto[]> GetTokens(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of Tokens
        /// </summary>
        /// <typeparam name="Token">Custom DTO</typeparam>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<Token[]> GetTokens<Token>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the counter of Tokens
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetTokensCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the specified Token
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <returns><see cref="TokenDto"/></returns>
        Task<TokenDto> GetToken(string tokenIdentifier);

        /// <summary>
        /// Returns the specified Token as custom object.
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <returns></returns>
        Task<Token> GetToken<Token>(string tokenIdentifier);

        /// <summary>
        /// Returns the accounts and balance for specified Token
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <returns><see cref="AccountTokenDto"/></returns>
        Task<AddressBalanceDto[]> GetTokenAccounts(string tokenIdentifier, int size = 100, int from = 0);

        /// <summary>
        /// Returns the number of accounts holding the specified Token
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <returns></returns>
        Task<string> GetTokenAccountsCount(string tokenIdentifier);
    }
}
