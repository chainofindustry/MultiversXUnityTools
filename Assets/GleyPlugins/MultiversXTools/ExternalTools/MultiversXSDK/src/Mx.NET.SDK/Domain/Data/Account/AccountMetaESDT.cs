using System;
using System.Linq;
using System.Text;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Account;

namespace Mx.NET.SDK.Domain.Data.Account
{
    public class AccountMetaESDT
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
        /// MetaESDT balance
        /// </summary>
        public ESDTAmount Balance { get; private set; }

        /// <summary>
        /// MetaESDT decimal precision
        /// </summary>
        public int Decimals { get; private set; }

        /// <summary>
        /// MetaESDT ticker
        /// </summary>
        public string Ticker { get; private set; }

        /// <summary>
        /// MetaESDT price
        /// </summary>
        public string Price { get; private set; }

        /// <summary>
        /// MetaESDT USD value of the balance owned
        /// </summary>
        public string ValueUSD { get; private set; }

        /// <summary>
        /// MetaESDT assets
        /// </summary>
        public dynamic Assets { get; private set; }

        private AccountMetaESDT() { }

        /// <summary>
        /// Creates a new AccountMetaESDT from data
        /// </summary>
        /// <param name="metaEsdt">MetaESDT Data Object from API</param>
        /// <returns>AccountMetaESDT object</returns>
        public static AccountMetaESDT From(AccountMetaESDTDto metaEsdt)
        {
            return new AccountMetaESDT()
            {
                Identifier = ESDTIdentifierValue.From(metaEsdt.Identifier),
                Collection = ESDTIdentifierValue.From(metaEsdt.Collection),
                Attributes = metaEsdt.Attributes is null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(metaEsdt.Attributes)),
                Nonce = metaEsdt.Nonce,
                Type = metaEsdt.Type,
                Name = metaEsdt.Name,
                Creator = Address.FromBech32(metaEsdt.Creator),
                IsWhitelistedStorage = metaEsdt.IsWhitelistedStorage,
                Balance = ESDTAmount.ESDT(metaEsdt.Balance,
                                          ESDT.META_ESDT(metaEsdt.Name, metaEsdt.Identifier, metaEsdt.Decimals)),
                Decimals = metaEsdt.Decimals,
                Ticker = metaEsdt.Ticker ?? metaEsdt.Identifier.GetTicker(),
                Price = metaEsdt.Price,
                ValueUSD = metaEsdt.ValueUSD,
                Assets = metaEsdt.Assets
            };
        }

        /// <summary>
        /// Creates a new array of AccountMetaESDT from data
        /// </summary>
        /// <param name="metaEsdt">Array of MetaESDT Data Objects from API</param>
        /// <returns>Array of AccountMetaESDT objects</returns>
        public static AccountMetaESDT[] From(AccountMetaESDTDto[] metaEsdts)
        {
            return metaEsdts.Select(metaEsdt => new AccountMetaESDT()
            {
                Identifier = ESDTIdentifierValue.From(metaEsdt.Identifier),
                Collection = ESDTIdentifierValue.From(metaEsdt.Collection),
                Attributes = metaEsdt.Attributes is null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(metaEsdt.Attributes)),
                Nonce = metaEsdt.Nonce,
                Type = metaEsdt.Type,
                Name = metaEsdt.Name,
                Creator = Address.FromBech32(metaEsdt.Creator),
                IsWhitelistedStorage = metaEsdt.IsWhitelistedStorage,
                Balance = ESDTAmount.From(metaEsdt.Balance,
                                          ESDT.META_ESDT(metaEsdt.Name, metaEsdt.Identifier, metaEsdt.Decimals)),
                Decimals = metaEsdt.Decimals,
                Ticker = metaEsdt.Ticker ?? metaEsdt.Identifier.GetTicker(),
                Price = metaEsdt.Price,
                ValueUSD = metaEsdt.ValueUSD,
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
