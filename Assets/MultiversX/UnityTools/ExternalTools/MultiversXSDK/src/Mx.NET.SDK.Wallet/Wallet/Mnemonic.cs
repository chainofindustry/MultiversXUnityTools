using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using dotnetstandard_bip39;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Wallet.Wallet
{
    public class Mnemonic
    {
        private const int MNEMONIC_STRENGTH = 256;
        private const string HdPrefix = "m/44'/508'/0'/0'";

        public string SeedPhrase { get; set; } = string.Empty;

        private Mnemonic(string phrase)
        {
            SeedPhrase = phrase;
        }

        public static Mnemonic Generate()
        {
            var bip39 = new BIP39();
            return new Mnemonic(bip39.GenerateMnemonic(MNEMONIC_STRENGTH, BIP39Wordlist.English));
        }

        public static Mnemonic GenerateInShard(int expectedShard)
        {
        GENERATE:
            var mnemonic = Generate();
            var publicKey = Wallet.FromMnemonic(mnemonic.SeedPhrase).GetPublicKey();
            var walletShard = new WalletPublicKey(publicKey).GetShard();
            if (walletShard != expectedShard)
                goto GENERATE;

            return mnemonic;
        }

        public static Mnemonic FromString(string seedPhrase)
        {
            seedPhrase = seedPhrase.Trim();

            var bip39 = new BIP39();
            if (bip39.ValidateMnemonic(seedPhrase, BIP39Wordlist.English))
                return new Mnemonic(seedPhrase);
            else
                throw new Exception("Bad mnemonic");
        }

        public string[] GetWords()
        {
            if (string.IsNullOrEmpty(SeedPhrase))
                throw new Exception("Seed phrase is empty");
            return SeedPhrase.Split(' ');
        }

        public static byte[] DecryptSecretKey(string mnemonic, int addressIndex = 0)
        {
            try
            {
                var bip39 = new BIP39();
                var seedHex = bip39.MnemonicToSeedHex(mnemonic, "");

                var hdPath = $"{HdPrefix}/{addressIndex}'";
                var kv = DerivePath(hdPath, seedHex);
                var secretKey = kv.Key;
                return secretKey;
            }
            catch (Exception ex)
            {
                throw new Exception("CannotDeriveKeysException", ex);
            }
        }

        private static (byte[] Key, byte[] ChainCode) DerivePath(string path, string seed)
        {
            var masterKeyFromSeed = GetMasterKeyFromSeed(seed);
            var segments = path
                          .Split('/')
                          .Skip(1)
                          .Select(a => a.Replace("'", ""))
                          .Select(a => Convert.ToUInt32(a, 10));

            var results = segments
               .Aggregate(masterKeyFromSeed, (mks, next) => GetChildKeyDerivation(mks.Key, mks.ChainCode, next + 0x80000000));

            return results;
        }

        private static (byte[] Key, byte[] ChainCode) GetMasterKeyFromSeed(string seed)
        {
            using (var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes("ed25519 seed")))
            {
                var i = hmacSha512.ComputeHash(Converter.FromHexString(seed));

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }

        private static (byte[] Key, byte[] ChainCode) GetChildKeyDerivation(byte[] key, byte[] chainCode, uint index)
        {
            var buffer = new BigEndianBuffer();

            buffer.Write(new byte[] { 0 });
            buffer.Write(key);
            buffer.WriteUInt(index);

            using (var hmacSha512 = new HMACSHA512(chainCode))
            {
                var i = hmacSha512.ComputeHash(buffer.ToArray());

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }
    }
}
