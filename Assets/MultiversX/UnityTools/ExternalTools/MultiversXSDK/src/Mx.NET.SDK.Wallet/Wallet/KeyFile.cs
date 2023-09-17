using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mx.NET.SDK.Core.Cryptography;
using Mx.NET.SDK.Core.Domain.Helper;
using Org.BouncyCastle.Crypto.Generators;
using static Mx.NET.SDK.Core.Domain.Constants.Constants;

namespace Mx.NET.SDK.Wallet.Wallet
{
    public class KeyFile
    {
        public int Version { get; set; }
        public string Id { get; set; }
        public string Address { get; set; }
        public string Bech32 { get; set; }
        public Crypto Crypto { get; set; }

        /// <summary>
        /// Load a KeyFile object from a json file path
        /// </summary>
        /// <param name="filePath">JSON String</param>
        /// <returns>KeyFile object</returns>
        private static KeyFile FromFilePath(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonWrapper.Deserialize<KeyFile>(json);
        }

        public static byte[] DecryptSecretKey(string filePath, string password)
        {
            var keyFile = FromFilePath(filePath);

            var saltBytes = Converter.FromHexString(keyFile.Crypto.Kdfparams.Salt);
            var kdParams = keyFile.Crypto.Kdfparams;
            var key = SCrypt.Generate(Encoding.UTF8.GetBytes(password), saltBytes, kdParams.N, kdParams.r,
                                      kdParams.p, kdParams.dklen);

            var rightPartOfKey = key.Skip(16).Take(16).ToArray();
            var leftPartOfKey = key.Take(16).ToArray();
            var mac = CreateSha256Signature(rightPartOfKey, keyFile.Crypto.Ciphertext);
            if (mac != keyFile.Crypto.Mac)
                throw new Exception("MAC mismatch, possibly wrong password");

            var decipher = EncryptAes128Ctr(Converter.FromHexString(keyFile.Crypto.Ciphertext),
                                            leftPartOfKey,
                                            Converter.FromHexString(keyFile.Crypto.Cipherparams.Iv));

            return Converter.FromHexString(decipher);
        }

        public static KeyFile BuildKeyFile(byte[] secretKey, byte[] publicKey, string password)
        {
            RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();
            var saltBytes = new byte[32];
            RngCsp.GetBytes(saltBytes);
            var ivBytes = new byte[16];
            RngCsp.GetBytes(ivBytes);

            var salt = Converter.ToHexString(saltBytes).ToLowerInvariant();
            var iv = Converter.ToHexString(ivBytes).ToLowerInvariant();
            var kdParams = new Crypto.KdfSructure
            {
                dklen = 32,
                Salt = salt,
                N = 4096,
                r = 8,
                p = 1
            };

            var encodedPassword = Encoding.UTF8.GetBytes(password);
            var key = SCrypt.Generate(encodedPassword, saltBytes, kdParams.N, kdParams.r, kdParams.p, kdParams.dklen);
            var leftPart = key.Take(16).ToArray();
            var rightPart = key.Skip(16).Take(16).ToArray();
            var cipher = EncryptAes128Ctr(secretKey, leftPart, ivBytes);
            var mac = CreateSha256Signature(rightPart, cipher);

            var keyFile = new KeyFile
            {
                Version = 4,
                Id = Guid.NewGuid().ToString(),
                Address = Converter.ToHexString(secretKey).ToLowerInvariant(),
                Bech32 = Bech32Engine.Encode(Hrp, publicKey),
                Crypto = new Crypto
                {
                    Ciphertext = cipher,
                    Cipherparams = new Crypto.CipherStructure() { Iv = iv },
                    Cipher = "aes-128-ctr",
                    Kdf = "scrypt",
                    Kdfparams = kdParams,
                    Mac = mac
                }
            };
            return keyFile;
        }

        private static string CreateSha256Signature(byte[] key, string targetText)
        {
            var data = Converter.FromHexString(targetText);
            byte[] mac;
            using (var hmac = new HMACSHA256(key))
            {
                mac = hmac.ComputeHash(data);
            }

            return Converter.ToHexString(mac).ToLowerInvariant();
        }

        private static string EncryptAes128Ctr(byte[] data, byte[] key, byte[] iv)
        {
            var output = AesCtr.Encrypt(key, iv, data);
            return Converter.ToHexString(output).ToLowerInvariant();
        }
    }

    public class Crypto
    {
        public string Ciphertext { get; set; }
        public CipherStructure Cipherparams { get; set; }
        public string Cipher { get; set; }
        public string Kdf { get; set; }
        public KdfSructure Kdfparams { get; set; }
        public string Mac { get; set; }

        public class CipherStructure
        {
            public string Iv { get; set; }
        }

        public class KdfSructure
        {
            public int dklen { get; set; }
            public string Salt { get; set; }
            public int N { get; set; }
            public int r { get; set; }
            public int p { get; set; }
        }
    }
}
