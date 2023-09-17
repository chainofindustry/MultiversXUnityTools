using System;
using System.Linq;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Wallet.Wallet;

namespace Mx.NET.SDK.Wallet
{
    public static class WalletMethods
    {
        /// <summary>
        /// Sign transaction
        /// </summary>
        /// <param name="signer">WalletSigner</param>
        /// <param name="txBuffer">Serialized transaction</param>
        /// <returns>Signature</returns>
        public static string SignTransaction(this WalletSigner signer, byte[] txBuffer)
        {
            return signer.Sign(txBuffer);
        }

        /// <summary>
        /// Sign a bunch of transactions
        /// </summary>
        /// <param name="signer">WalletSigner</param>
        /// <param name="txBuffer">Serialized transactions</param>
        /// <returns>Array of signatures</returns>
        public static string[] SignTransactions(this WalletSigner signer, byte[][] txsBuffer)
        {
            return txsBuffer.Select(buffer => signer.Sign(buffer)).ToArray();
        }

        /// <summary>
        /// Verify signature in the context of Transaction
        /// </summary>
        /// <param name="txData">Serialized JSON of one transaction</param>
        /// <param name="address">Wallet address</param>
        /// <param name="signature">Signature</param>
        /// <returns></returns>
        public static bool VerifySignature(this string txData, string address, string signature)
        {
            var verifier = WalletVerifier.FromAddress(Address.FromBech32(address));
            return verifier.VerifyRaw(new SignableMessage()
            {
                Message = txData,
                Signature = signature
            });
        }

        /// <summary>
        /// Verify a signed message
        /// </summary>
        /// <param name="signableMessage">Signable Message object</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool VerifyMessage(this SignableMessage signableMessage)
        {
            if (signableMessage.Address is null)
                throw new Exception("Address is not initialized");

            var verifier = WalletVerifier.FromAddress(signableMessage.Address);
            return verifier.Verify(signableMessage);
        }
    }
}
