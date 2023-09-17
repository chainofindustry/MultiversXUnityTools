namespace Mx.NET.SDK.Domain.SDKConstants
{
    /// <summary>
    /// Constant parameters for Non-Fungible/Semi-Fungible/MetaESDT Tokens
    /// </summary>
    public static class ESDTConstants
    {
        /// <summary>
        /// Constant Account roles for a Non-Fungible/Semi-Fungible/MetaESDT Token collection
        /// </summary>
        public static class ESDTCollectionAccountRoles
        {
            /// <summary>
            /// This role allows one to create a new NFT/SFT/MetaESDT
            /// </summary>
            public const string ESDTRoleNFTCreate = "ESDTRoleNFTCreate";

            /// <summary>
            /// This role allows one to burn quantity of a specific NFT/SFT/MetaESDT
            /// </summary>
            public const string ESDTRoleNFTBurn = "ESDTRoleNFTBurn";

            /// <summary>
            /// This role allows one to add quantity of a specific SFT/MetaESDT
            /// </summary>
            public const string ESDTRoleNFTAddQuantity = "ESDTRoleNFTAddQuantity";

            /// <summary>
            /// This role allows one to update NFT/SFT attributes
            /// </summary>
            public const string ESDTRoleNFTUpdateAttributes = "ESDTRoleNFTUpdateAttributes";

            /// <summary>
            /// This role allows one to update NFT/SFT URIs
            /// </summary>
            public const string ESDTRoleNFTAddURI = "ESDTRoleNFTAddURI";
        }

        /// <summary>
        /// Constant properties for a Non-Fungible/Semi-Fungible/MetaESDT Token collection
        /// </summary>
        public static class ESDTCollectionProperties
        {
            public const string CanFreeze = "canFreeze";
            public const string CanWipe = "canWipe";
            public const string CanPause = "canPause";
            public const string CanTransferNftCreateRole = "canTransferNFTCreateRole";

            public const string CanChangeOwner = "canChangeOwner";
            public const string CanUpgrade = "canUpgrade";
            public const string CanAddSpecialRoles = "canAddSpecialRoles";
        }
    }
}
