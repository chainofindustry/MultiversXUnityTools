using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Account;
using Org.BouncyCastle.Crypto.Parameters;

namespace Mx.NET.SDK.Wallet.Wallet
{
    public class Wallet
    {
        private readonly byte[] _secretKey;
        private readonly byte[] _publicKey;

        public Wallet(string secretKeyHex) : this(Converter.FromHexString(secretKeyHex)) { }

        /// <summary>
        /// Build a wallet
        /// </summary>
        /// <param name="secretKey">The secret key</param>
        public Wallet(byte[] secretKey)
        {
            var privateKeyParameters = new Ed25519PrivateKeyParameters(secretKey, 0);
            var publicKeyParameters = privateKeyParameters.GeneratePublicKey();
            _publicKey = publicKeyParameters.GetEncoded();
            _secretKey = secretKey;
        }

        /// <summary>
        /// Derive a wallet from Mnemonic phrase
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase</param>
        /// <param name="accountIndex">The account index, default 0</param>
        /// <returns>Wallet</returns>
        public static Wallet FromMnemonic(string mnemonic, int accountIndex = 0)
        {
            return new Wallet(Mnemonic.DecryptSecretKey(mnemonic, accountIndex));
        }

        /// <summary>
        /// Derive a wallet from a KeyFile and a password
        /// </summary>
        /// <param name="filePath">The KeyFile path</param>
        /// <param name="password">The password</param>
        /// <returns>Wallet</returns>
        public static Wallet FromKeyFile(string filePath, string password)
        {
            return new Wallet(KeyFile.DecryptSecretKey(filePath, password));
        }

        /// <summary>
        /// Derive a wallet from PemFile
        /// </summary>
        /// <param name="filePath">The PemFile path</param>
        /// <returns>Wallet</returns>
        public static Wallet FromPemFile(string filePath)
        {
            return new Wallet(PemFile.DecryptSecretKey(filePath));
        }

        /// <summary>
        /// Get the account wallet
        /// </summary>
        /// <returns>Account</returns>
        public Account GetAccount()
        {
            return new Account(Address.FromBytes(_publicKey));
        }

        /// <summary>
        /// Get the wallet signer
        /// </summary>
        /// <returns>Signer</returns>
        public Signer GetSigner()
        {
            return new Signer(_secretKey);
        }

        /// <summary>
        /// The secret key. Do not share
        /// </summary>
        /// <returns>Secret key</returns>
        public byte[] GetSecretKey()
        {
            return _secretKey;
        }

        /// <summary>
        /// The public key
        /// </summary>
        /// <returns>Public key</returns>
        public byte[] GetPublicKey()
        {
            return _publicKey;
        }

        /// <summary>
        /// Build a key file from existing wallet with a new password
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>KeyFile</returns>
        public KeyFile BuildKeyFile(string password)
        {
            return KeyFile.BuildKeyFile(_secretKey, _publicKey, password);
        }

        /// <summary>
        /// Serialize a key file which can be saved as json file. Then can be accessed with json + password
        /// </summary>
        /// <param name="keyFile">KeyFile builded with 'BuildKeyFile' function</param>
        /// <returns>string to save</returns>
        public string SerializeKeyFile(KeyFile keyFile)
        {
            return JsonWrapper.Serialize(keyFile);
        }
    }
}
