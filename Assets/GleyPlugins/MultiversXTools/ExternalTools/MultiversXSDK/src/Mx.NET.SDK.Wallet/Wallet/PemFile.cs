using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Wallet.Wallet
{
    public class PemFile
    {
        private static string FromFilePath(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return string.Join("", lines.Where(l => !l.StartsWith("-----")));
        }

        private static List<byte[]> ParseKeys(string text)
        {
            var bytes = Converter.FromHexString(Encoding.UTF8.GetString(Convert.FromBase64String(text)));
            return new List<byte[]>()
            {
                bytes.Take(32).ToArray(),
                bytes.Skip(32).Take(32).ToArray()
            };
        }

        public static byte[] DecryptSecretKey(string filePath, int index = 0)
        {
            var text = FromFilePath(filePath);

            var keys = ParseKeys(text);
            return keys[index];
        }

        public static string BuildPemFile(WalletSecretKey secretKey)
        {
            throw new NotImplementedException();

            var publicKey = secretKey.GeneratePublicKey();
            var address = publicKey.ToAddress().Bech32;
            string header = $"-----BEGIN PRIVATE KEY for {address}-----";
            string footer = $"-----END PRIVATE KEY for {address}-----";

            var hex = Converter.ToHexString(secretKey.GetKey().Concat(publicKey.GetKey()).ToArray());
            var text = Convert.ToBase64String(Encoding.UTF8.GetBytes(hex));
            return text;
        }
    }
}
