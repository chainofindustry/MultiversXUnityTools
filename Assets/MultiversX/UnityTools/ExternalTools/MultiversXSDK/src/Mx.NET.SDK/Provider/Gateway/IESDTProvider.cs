using Mx.NET.SDK.Provider.Dtos.Gateway.ESDTs;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.Gateway
{
    public interface IESDTProvider
    {
        /// <summary>
        /// Returns an array of FungibleESDT Tokens that the specified address has interacted with (issued, sent or received).
        /// </summary>
        /// <param name="address">The Addresses to query in bech32 format.</param>
        /// <returns><see cref="EsdtTokenDataDto"/></returns>
        Task<EsdtTokenDataDto> GetEsdtTokens(string address);

        /// <summary>
        /// Returns the balance of an address for specific ESDT Tokens.
        /// </summary>
        /// <param name="address">The Addresses to query in bech32 format.</param>
        /// <param name="tokenIdentifier">The token identifier.</param>
        /// <returns><see cref="EsdtTokenData"/></returns>
        Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier);
    }
}
