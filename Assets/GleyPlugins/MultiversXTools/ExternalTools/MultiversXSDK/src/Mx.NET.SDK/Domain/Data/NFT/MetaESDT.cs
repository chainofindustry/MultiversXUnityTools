using System;
using System.Linq;
using System.Text;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Helper;
using Mx.NET.SDK.Provider.Dtos.API.NFT;

namespace Mx.NET.SDK.Domain.Data.NFT
{
    public class MetaESDT
    {
        /// <summary>
        /// MetaESDT Identifier
        /// </summary>
        public ESDTIdentifierValue Identifier { get; private set; }

        /// <summary>
        /// MetaESDT collection identifier
        /// </summary>
        public ESDTIdentifierValue Collection { get; private set; }

        /// <summary>
        /// MetaESDT creation date
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// MetaESDT attributes
        /// </summary>
        public string Attributes { get; private set; }

        /// <summary>
        /// MetaESDT Nonce (ID in decimal)
        /// </summary>
        public ulong Nonce { get; private set; }

        /// <summary>
        /// The ESDT type: Meta ESDT
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// MetaESDT name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// MetaESDT creator
        /// </summary>
        public Address Creator { get; private set; }

        /// <summary>
        /// Whitelisted (decentralized) storage bool
        /// </summary>
        public bool IsWhitelistedStorage { get; private set; }

        /// <summary>
        /// MetaESDT supply
        /// </summary>
        public string Supply { get; private set; }

        /// <summary>
        /// MetaESDT decimal precision
        /// </summary>
        public int Decimals { get; private set; }

        /// <summary>
        /// MetaESDT ticker
        /// </summary>
        public string Ticker { get; private set; }

        /// <summary>
        /// MetaESDT assets
        /// </summary>
        public dynamic Assets { get; private set; }

        private MetaESDT() { }

        /// <summary>
        /// Creates a new MetaESDT from data
        /// </summary>
        /// <param name="metaEsdt">MetaESDT Data Object from API</param>
        /// <returns>MetaESDT object</returns>
        public static MetaESDT From(MetaESDTDto metaEsdt)
        {
            return new MetaESDT()
            {
                Identifier = ESDTIdentifierValue.From(metaEsdt.Identifier),
                Collection = ESDTIdentifierValue.From(metaEsdt.Collection),
                CreationDate = metaEsdt.Timestamp.ToDateTime(),
                Attributes = metaEsdt.Attributes is null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(metaEsdt.Attributes)),
                Nonce = metaEsdt.Nonce,
                Type = metaEsdt.Type,
                Name = metaEsdt.Name,
                Creator = Address.FromBech32(metaEsdt.Creator),
                IsWhitelistedStorage = metaEsdt.IsWhitelistedStorage,
                Supply = metaEsdt.Supply,
                Decimals = metaEsdt.Decimals,
                Ticker = metaEsdt.Ticker ?? metaEsdt.Identifier.GetTicker(),
                Assets = metaEsdt.Assets
            };
        }

        /// <summary>
        /// Creates a new array of MetaESDTs from data
        /// </summary>
        /// <param name="metaEsdts">Array of MetaESDT Data Objects from API</param>
        /// <returns>Array of MetaESDT objects</returns>
        public static MetaESDT[] From(MetaESDTDto[] metaEsdts)
        {
            return metaEsdts.Select(metaEsdt => new MetaESDT()
            {
                Identifier = ESDTIdentifierValue.From(metaEsdt.Identifier),
                Collection = ESDTIdentifierValue.From(metaEsdt.Collection),
                CreationDate = metaEsdt.Timestamp.ToDateTime(),
                Attributes = metaEsdt.Attributes is null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(metaEsdt.Attributes)),
                Nonce = metaEsdt.Nonce,
                Type = metaEsdt.Type,
                Name = metaEsdt.Name,
                Creator = Address.FromBech32(metaEsdt.Creator),
                IsWhitelistedStorage = metaEsdt.IsWhitelistedStorage,
                Supply = metaEsdt.Supply,
                Decimals = metaEsdt.Decimals,
                Ticker = metaEsdt.Ticker ?? metaEsdt.Identifier.GetTicker(),
                Assets = metaEsdt.Assets
            }).ToArray();
        }

        /// <summary>
        /// Get the ESDT object from MetaESDT
        /// </summary>
        /// <returns></returns>
        public ESDT GetESDT()
        {
            return ESDT.META_ESDT(Name, Identifier.Value, Decimals);
        }
    }
}
