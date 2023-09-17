using System;
using System.Linq;
using System.Text;
using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Accounts;
using Mx.NET.SDK.Core.Domain;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    /// <summary>
    /// Account NFT object
    /// </summary>
    public class AccountNFT
    {
        /// <summary>
        /// NFT/SFT Identifier
        /// </summary>
        public ESDTIdentifierValue Identifier { get; private set; }

        /// <summary>
        /// NFT/SFT collection identifier
        /// </summary>
        public ESDTIdentifierValue Collection { get; private set; }

        /// <summary>
        /// NFT/SFT attributes
        /// </summary>
        public string Attributes { get; private set; }

        /// <summary>
        /// NFT/SFT Nonce (ID in decimal)
        /// </summary>
        public ulong Nonce { get; private set; }

        /// <summary>
        /// The NFT type: NonFungibleESDT/SemiFungibleESDT
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// NFT/SFT name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// NFT/SFT creator
        /// </summary>
        public Address Creator { get; private set; }

        /// <summary>
        /// NFT/SFT royalties for creator
        /// </summary>
        public float Royalties { get; private set; }

        /// <summary>
        /// NFT/SFT URIs
        /// </summary>
        public Uri[] Uris { get; private set; }

        /// <summary>
        /// NFT/SFT URL
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// NFT/SFT media
        /// </summary>
        public dynamic Media { get; private set; }

        /// <summary>
        /// Whitelisted (decentralized) storage bool
        /// </summary>
        public bool IsWhitelistedStorage { get; private set; }

        /// <summary>
        /// NFT/SFT tags
        /// </summary>
        public string[] Tags { get; private set; }

        /// <summary>
        /// NFT/SFT metadata
        /// </summary>
        public dynamic Metadata { get; private set; }

        /// <summary>
        /// NFT/SFT balance
        /// </summary>
        public string Balance { get; private set; }

        /// <summary>
        /// NFT/SFT ticker
        /// </summary>
        public string Ticker { get; private set; }

        /// <summary>
        /// NFT/SFT scam info
        /// </summary>
        public ScamInfo ScamInfo { get; private set; }

        /// <summary>
        /// Is Not Safe For Watch
        /// </summary>
        public bool IsNSFW { get; private set; }

        /// <summary>
        /// NFT/SFT Assets
        /// </summary>
        public dynamic Assets { get; private set; }

        private AccountNFT() { }

        /// <summary>
        /// Creates a new AccountNFT object from data
        /// </summary>
        /// <param name="nft">NFT Data Object from API</param>
        /// <returns>AccountNFT object</returns>
        public static AccountNFT From(AccountNftDto nft)
        {
            return new AccountNFT()
            {
                Identifier = ESDTIdentifierValue.From(nft.Identifier),
                Collection = ESDTIdentifierValue.From(nft.Collection),
                Attributes = nft.Attributes is null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(nft.Attributes)),
                Nonce = nft.Nonce,
                Type = nft.Type,
                Name = nft.Name,
                Creator = Address.FromBech32(nft.Creator),
                Royalties = nft.Royalties,
                Uris = nft.URIs.Select(u => new UriBuilder(Encoding.UTF8.GetString(Convert.FromBase64String(u))).Uri)
                               .ToArray(),
                Url = new Uri(nft.URL),
                Media = nft.Media,
                IsWhitelistedStorage = nft.IsWhitelistedStorage,
                Tags = nft.Tags,
                Metadata = nft.Metadata,
                Balance = nft.Balance,
                Ticker = nft.Ticker ?? nft.Identifier.GetTicker(),
                ScamInfo = ScamInfo.From(nft.ScamInfo),
                IsNSFW = nft.IsNSFW,
                Assets = nft.Assets
            };
        }

        /// <summary>
        /// Creates a new array of AccountNFT objects from data
        /// </summary>
        /// <param name="nfts">Array of NFT Data Objects from API</param>
        /// <returns>Array of AccountNFT objects</returns>
        public static AccountNFT[] From(AccountNftDto[] nfts)
        {
            return nfts.Select(nft => new AccountNFT()
            {
                Identifier = ESDTIdentifierValue.From(nft.Identifier),
                Collection = ESDTIdentifierValue.From(nft.Collection),
                Attributes = nft.Attributes is null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(nft.Attributes)),
                Nonce = nft.Nonce,
                Type = nft.Type,
                Name = nft.Name,
                Creator = Address.FromBech32(nft.Creator),
                Royalties = nft.Royalties,
                Uris = nft.URIs.Select(u => new UriBuilder(Encoding.UTF8.GetString(Convert.FromBase64String(u))).Uri)
                                                        .ToArray(),
                Url = new Uri(nft.URL),
                Media = nft.Media,
                IsWhitelistedStorage = nft.IsWhitelistedStorage,
                Tags = nft.Tags,
                Metadata = nft.Metadata,
                Balance = nft.Balance,
                Ticker = nft.Ticker ?? nft.Identifier.GetTicker(),
                ScamInfo = ScamInfo.From(nft.ScamInfo),
                IsNSFW = nft.IsNSFW,
                Assets = nft.Assets
            }).ToArray();
        }

        /// <summary>
        /// Get the ESDT object from NFT/SFT
        /// </summary>
        /// <returns></returns>
        public ESDT GetESDT()
        {
            return ESDT.NFT(Name, Identifier.Value);
        }
    }
}
