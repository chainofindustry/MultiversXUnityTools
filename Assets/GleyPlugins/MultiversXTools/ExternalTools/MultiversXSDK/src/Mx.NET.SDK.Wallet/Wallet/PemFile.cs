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
            var publicKey = secretKey.GeneratePublicKey();
            var address = publicKey.ToAddress().Bech32;
            string header = $"-----BEGIN PRIVATE KEY for {address}-----";
            string footer = $"-----END PRIVATE KEY for {address}-----";

            var keys = secretKey.GetKey().Concat(publicKey.GetKey()).ToArray();
            var hex = Encoding.UTF8.GetBytes(Converter.ToHexString(keys));
            var base64Key = Convert.ToBase64String(hex);

            var lines = SplitBy(base64Key, 64);
            var text = string.Join(Environment.NewLine, lines);
            return string.Join(Environment.NewLine, new[] { header, text, footer });
        }

        private static IEnumerable<string> SplitBy(string text, int chunkSize)
        {
            var chunks = new List<string>();
            for (var i = 0; i < text.Length; i += chunkSize)
            {
                var chunk = text.Substring(i, Math.Min(chunkSize, text.Length - i));
                chunks.Add(chunk);
            }
            return chunks;
        }
    }
}
