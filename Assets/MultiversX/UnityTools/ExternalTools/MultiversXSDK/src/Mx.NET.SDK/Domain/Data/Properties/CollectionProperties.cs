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
}
