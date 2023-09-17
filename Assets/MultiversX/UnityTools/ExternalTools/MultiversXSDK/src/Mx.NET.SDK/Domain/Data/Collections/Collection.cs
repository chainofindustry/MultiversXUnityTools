using System;
using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Domain.Data.Properties;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Collections;
using Mx.NET.SDK.Domain.Helper;

namespace Mx.NET.SDK.Domain.Data.Collections
{
    public class Collection
    {
        /// <summary>
        /// Collection identifier
        /// </summary>
        public ESDTIdentifierValue CollectionIdentifier { get; private set; }

        /// <summary>
        /// Collection type (NFT/SFT/MetaESDT)
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Collection name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Collection ticker
        /// </summary>
        public string Ticker { get; private set; }

        /// <summary>
        /// Collection owner
        /// </summary>
        public Address Owner { get; private set; }

        /// <summary>
        /// Collection creation date
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Collection properties
        /// </summary>
        public CollectionProperties Properties { get; private set; }

        /// <summary>
        /// Collection decimal precision (only for MetaESDT)
        /// </summary>
        public int Decimals { get; private set; }

        /// <summary>
        /// Collection assets
        /// </summary>
        public dynamic Assets { get; private set; }

        /// <summary>
        /// Collection roles for addresses
        /// </summary>
        public CollectionRoles[] Roles { get; private set; }

        /// <summary>
        /// Collection scam info
        /// </summary>
        public ScamInfo ScamInfo { get; private set; }

        private Collection() { }

        /// <summary>
        /// Creates a new Collection from data
        /// </summary>
        /// <param name="collection">Collection Data Object from API</param>
        /// <returns>Collection object</returns>
        public static Collection From(CollectionDto collection)
        {
            return new Collection()
            {
                CollectionIdentifier = ESDTIdentifierValue.From(collection.Collection),
                Type = collection.Type,
                Name = collection.Name,
                Ticker = collection.Ticker,
                Owner = Address.From(collection.Owner),
                CreationDate = collection.Timestamp.ToDateTime(),
                Properties = CollectionProperties.From(collection.CanFreeze,
                                                       collection.CanWipe,
                                                       collection.CanPause,
                                                       collection.CanTransferNFTCreateRole,
                                                       collection.CanChangeOwner,
                                                       collection.CanUpgrade,
                                                       collection.CanAddSpecialRoles),
                Decimals = collection.Decimals,
                Roles = CollectionRoles.From(collection.Roles),
                ScamInfo = ScamInfo.From(collection.ScamInfo)
            };
        }
    }
}
