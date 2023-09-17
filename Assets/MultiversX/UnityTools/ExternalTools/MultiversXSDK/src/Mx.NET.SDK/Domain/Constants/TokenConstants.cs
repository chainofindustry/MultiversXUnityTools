namespace Mx.NET.SDK.Domain.SDKConstants
{
    /// <summary>
    /// Constant parameters for a Token
    /// </summary>
    public static class TokenConstants
    {
        /// <summary>
        /// Constant Account roles for a Token collection
        /// </summary>
        public static class ESDTTokenAccountRoles
        {
            /// <summary>
            /// This role allows one to mint a supply of tokens locally
            /// </summary>
            public const string ESDTRoleLocalMint = "ESDTRoleLocalMint";

            /// <summary>
            /// This role allows one to burn a supply of tokens locally
            /// </summary>
            public const string ESDTRoleLocalBurn = "ESDTRoleLocalBurn";

            /// <summary>
            /// This role restricts transferability of the token only to the addresses that have the role set, while these addresses can send to any address
            /// </summary>
            public const string ESDTTransferRole = "ESDTTransferRole";
        }

        /// <summary>
        /// Constant properties for a Token collection
        /// </summary>
        public static class ESDTTokenProperties
        {
            public const string CanFreeze = "canFreeze";
            public const string CanWipe = "canWipe";
            public const string CanPause = "canPause";
            public const string CanMint = "canMint";
            public const string CanBurn = "canBurn";
            public const string CanChangeOwner = "canChangeOwner";
            public const string CanUpgrade = "canUpgrade";
            public const string CanAddSpecialRoles = "canAddSpecialRoles";
        }
    }
}
