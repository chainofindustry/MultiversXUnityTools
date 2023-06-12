using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Wallet.Wallet
{
    public class WalletVerifier
    {
        private readonly WalletPublicKey _publicKey;

        public WalletVerifier(byte[] publicKey)
        {
            _publicKey = new WalletPublicKey(publicKey);
        }

        public WalletVerifier(WalletPublicKey publicKey)
        {
            _publicKey = publicKey;
        }

        public static WalletVerifier FromAddress(Address address)
        {
            return new WalletVerifier(address.PublicKey());
        }

        public bool Verify(SignableMessage message)
        {
            return _publicKey.Verify(message.SerializeForSigning(),
                                     Converter.FromHexString(message.Signature));
        }

        public bool VerifyRaw(SignableMessage message)
        {
            return _publicKey.Verify(message.SerializeForSigningRaw(),
                                     Converter.FromHexString(message.Signature));
        }
    }
}
