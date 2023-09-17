using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System;

namespace Mx.NET.SDK.Wallet.Wallet
{
    public class WalletSecretKey
    {
        private readonly byte[] _secretKey;

        public WalletSecretKey(byte[] secretKey)
        {
            _secretKey = secretKey;
        }

        /// <summary>
        /// The private key. Do not share
        /// </summary>
        /// <returns>Private key</returns>
        public byte[] GetKey()
        {
            return _secretKey;
        }

        public WalletPublicKey GeneratePublicKey()
        {
            var privateKeyParameters = new Ed25519PrivateKeyParameters(_secretKey, 0);
            var publicKeyParameters = privateKeyParameters.GeneratePublicKey();
            return new WalletPublicKey(publicKeyParameters.GetEncoded());
        }

        /// <summary>
        /// Derive a signer from Mnemonic phrase
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase</param>
        /// <param name="accountIndex">The account index, default 0</param>
        /// <returns></returns>
        public static WalletSecretKey FromMnemonic(string mnemonic, int accountIndex = 0)
        {
            return new WalletSecretKey(Mnemonic.DecryptSecretKey(mnemonic, accountIndex));
        }

        /// <summary>
        /// Derive a signer from a KeyFile and a password
        /// </summary>
        /// <param name="filePath">The KeyFile path</param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        public static WalletSecretKey FromKeyFile(string filePath, string password)
        {
            return new WalletSecretKey(KeyFile.DecryptSecretKey(filePath, password));
        }

        /// <summary>
        /// Derive a signer from PemFile
        /// </summary>
        /// <param name="filePath">The PemFile path</param>
        /// <returns></returns>
        public static WalletSecretKey FromPemFile(string filePath)
        {
            return new WalletSecretKey(PemFile.DecryptSecretKey(filePath));
        }

        public string Sign(byte[] data)
        {
            var parameters = new Ed25519PrivateKeyParameters(_secretKey, 0);
            var signer = new Ed25519Signer();
            signer.Init(true, parameters);
            signer.BlockUpdate(data, 0, data.Length);
            var signature = signer.GenerateSignature();
            return Converter.ToHexString(signature).ToLowerInvariant();
        }
    }

    public class WalletPublicKey
    {
        private readonly byte[] _publicKey;

        public WalletPublicKey(byte[] publicKey)
        {
            _publicKey = publicKey;
        }

        /// <summary>
        /// The public key
        /// </summary>
        /// <returns>Public key</returns>
        public byte[] GetKey()
        {
            return _publicKey;
        }

        public WalletPublicKey From(Address address)
        {
            return new WalletPublicKey(address.PublicKey());
        }

        public Address ToAddress()
        {
            return Address.FromBytes(_publicKey);
        }

        public int GetShard()
        {
            int startingIndex = _publicKey.Length - 1;
            byte[] usedBuffer = new byte[_publicKey.Length - startingIndex];
            Array.Copy(_publicKey, startingIndex, usedBuffer, 0, usedBuffer.Length);

            int addr = 0;
            for (int i = 0; i < usedBuffer.Length; i++)
            {
                addr = (addr << 8) + usedBuffer[i];
            }

            int n = (int)Math.Ceiling(Math.Log(3, 2));
            int maskHigh = (1 << n) - 1;
            int maskLow = (1 << (n - 1)) - 1;

            int shard = addr & maskHigh;
            if (shard > 2)
            {
                shard = addr & maskLow;
            }

            return shard;
        }

        public bool Verify(byte[] data, byte[] signature)
        {
            var parameters = new Ed25519PublicKeyParameters(_publicKey, 0);
            var signer = new Ed25519Signer();
            signer.Init(false, parameters);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }
    }
}
