namespace Mx.NET.SDK.Domain.Data.Properties
{
    public class CollectionProperties
    {
        public bool CanFreeze { get; private set; }
        public bool CanWipe { get; private set; }
        public bool CanPause { get; private set; }
        public bool CanTransferNFTCreateRole { get; private set; }
        public bool CanChangeOwner { get; private set; }
        public bool CanUpgrade { get; private set; }
        public bool CanAddSpecialRoles { get; private set; }

        private CollectionProperties(bool canFreeze,
                                     bool canWipe,
                                     bool canPause,
                                     bool canTransferNFTCreateRole,
                                     bool canChangeOwner,
                                     bool canUpgrade,
                                     bool canAddSpecialRoles)
        {
            CanFreeze = canFreeze;
            CanWipe = canWipe;
            CanPause = canPause;
            CanTransferNFTCreateRole = canTransferNFTCreateRole;
            CanChangeOwner = canChangeOwner;
            CanUpgrade = canUpgrade;
            CanAddSpecialRoles = canAddSpecialRoles;
        }

        /// <summary>
        /// The Collection Properties
        /// </summary>
        /// <param name="canFreeze">The collection manager may freeze the NFT/SFT/MetaESDT in a specific account, preventing transfers to and from that account</param>
        /// <param name="canWipe">The collection manager may wipe out the NFT/SFT/MetaESDT held by a frozen account, reducing the supply</param>
        /// <param name="canPause">The collection manager may prevent all transactions of an NFT</param>
        /// <param name="canTransferNFTCreateRole">The collection manager may transfer the role of creating an NFT</param>
        /// <returns>The manager properties of the collection</returns>
        public static CollectionProperties From(bool canFreeze,
                                                bool canWipe,
                                                bool canPause,
                                                bool canTransferNFTCreateRole,
                                                bool canChangeOwner,
                                                bool canUpgrade,
                                                bool canAddSpecialRoles)
        {
            return new CollectionProperties(canFreeze, canWipe, canPause, canTransferNFTCreateRole, canChangeOwner, canUpgrade, canAddSpecialRoles);
        }
    }

    public class CollectionManagerProperties
    {
        public bool? CanCreate { get; private set; }
        public bool? CanBurn { get; private set; }
        public bool? CanUpdateAttributes { get; private set; }
        public bool? CanAddURI { get; private set; }
        public bool? CanTransfer { get; private set; }
        public bool? CanAddQuantity { get; private set; }

        private CollectionManagerProperties() { }

        /// <summary>
        /// The Collection Manager Properties
        /// </summary>
        /// <param name="canCreate">The collection manager is allowed to create a new NFT/SFT/MetaESDT</param>
        /// <param name="canBurn">The collection manager is allowed to burn quantity of a specific NFT/SFT/MetaESDT</param>
        /// <param name="canUpdateAttributes">The collection manager is allowed to update attributes of a specific NFT/SFT/MetaESDT</param>
        /// <param name="canAddURI">The collection manager is allowed add URIs of a specific NFT/SFT/MetaESDT</param>
        /// <param name="canTransfer">This role enables transfer only to specified addresses. The owner of the NFT and the address with the ESDTTransferRole should be located on the same shard. The addresses with the transfer role can transfer anywhere.</param>
        /// <param name="canAddQuantity">The collection manager is allowed add quantity of a specific SFT</param>
        /// <returns>The collection manager properties</returns>
        public static CollectionManagerProperties From(bool? canCreate,
                                                       bool? canBurn,
                                                       bool? canUpdateAttributes,
                                                       bool? canAddURI,
                                                       bool? canTransfer,
                                                       bool? canAddQuantity)
        {
            return new CollectionManagerProperties()
            {
                CanCreate = canCreate,
                CanBurn = canBurn,
                CanUpdateAttributes = canUpdateAttributes,
                CanAddURI = canAddURI,
                CanTransfer = canTransfer,
                CanAddQuantity = canAddQuantity
            };
        }
    }
}
