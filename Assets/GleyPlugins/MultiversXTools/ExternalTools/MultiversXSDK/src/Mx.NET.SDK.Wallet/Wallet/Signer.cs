using System;
using System.Text;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Wallet.Wallet
{
    public class Signer
    {
        private WalletSecretKey _secretKey;

        public Signer(byte[] secretKey)
        {
            _secretKey = new WalletSecretKey(secretKey);
        }

        public Signer(WalletSecretKey secretKey)
        {
            _secretKey = secretKey;
        }

        /// <summary>
        /// Derive a signer from Mnemonic phrase
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase</param>
        /// <param name="accountIndex">The account index, default 0</param>
        /// <returns></returns>
        public static Signer FromMnemonic(string mnemonic, int accountIndex = 0)
        {
            return new Signer(Mnemonic.DecryptSecretKey(mnemonic, accountIndex));
        }

        /// <summary>
        /// Derive a signer from a KeyFile and a password
        /// </summary>
        /// <param name="filePath">The KeyFile path</param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        public static Signer FromKeyFile(string filePath, string password)
        {
            return new Signer(KeyFile.DecryptSecretKey(filePath, password));
        }

        /// <summary>
        /// Derive a signer from PemFile
        /// </summary>
        /// <param name="filePath">The PemFile path</param>
        /// <returns></returns>
        public static Signer FromPemFile(string filePath)
        {
            return new Signer(PemFile.DecryptSecretKey(filePath));
        }

        /// <summary>
        /// Sign data string with the wallet
        /// </summary>
        /// <param name="data">The data to signed</param>
        /// <returns>Signature</returns>
        public string Sign(string data)
        {
            return Sign(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sign data with the wallet
        /// </summary>
        /// <param name="data">The data to signed</param>
        /// <returns>Signature</returns>
        public string Sign(byte[] data)
        {
            return _secretKey.Sign(data);
        }

        public Address GetAddress()
        {
            return _secretKey.GeneratePublicKey().ToAddress();
        }
    }
}
