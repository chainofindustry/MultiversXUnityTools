namespace Mx.NET.SDK.Domain.Data.Properties
{
    public class TokenProperties
    {
        public bool CanFreeze { get; private set; }
        public bool CanWipe { get; private set; }
        public bool CanPause { get; private set; }
        public bool CanMint { get; private set; }
        public bool CanBurn { get; private set; }
        public bool CanUpgrade { get; private set; }
        public bool CanChangeOwner { get; private set; }
        public bool CanAddSpecialRoles { get; private set; }

        private TokenProperties() { }

        private TokenProperties(bool canFreeze,
                                bool canWipe,
                                bool canPause,
                                bool canMint,
                                bool canBurn,
                                bool canUpgrade,
                                bool canChangeOwner,
                                bool canAddSpecialRoles)
        {
            CanFreeze = canFreeze;
            CanWipe = canWipe;
            CanPause = canPause;
            CanMint = canMint;
            CanBurn = canBurn;
            CanUpgrade = canUpgrade;
            CanChangeOwner = canChangeOwner;
            CanAddSpecialRoles = canAddSpecialRoles;
        }

        /// <summary>
        /// Creates a new Token Properties object
        /// </summary>
        /// <param name="canFreeze">The token manager may freeze the token balance in a specific account, preventing transfers to and from that account</param>
        /// <param name="canWipe">The token manager may wipe out the tokens held by a frozen account, reducing the supply</param>
        /// <param name="canPause">The token manager may prevent all transactions of the token, apart from minting and burning</param>
        /// <param name="canMint">More units of this token can be minted by the token manager after initial issuance, increasing the supply</param>
        /// <param name="canBurn">Users may "burn" some of their tokens, reducing the supply</param>
        /// <param name="canUpgrade">The token manager may change these properties</param>
        /// <param name="canChangeOwner">Token management can be transferred to a different account</param>
        /// <returns>The token properties</returns>
        public static TokenProperties From(bool canFreeze,
                                           bool canWipe,
                                           bool canPause,
                                           bool canMint,
                                           bool canBurn,
                                           bool canUpgrade,
                                           bool canChangeOwner,
                                           bool canAddSpecialRoles)
        {
            return new TokenProperties(canFreeze, canWipe, canPause, canMint, canBurn, canUpgrade, canChangeOwner, canAddSpecialRoles);
        }
    }
}
