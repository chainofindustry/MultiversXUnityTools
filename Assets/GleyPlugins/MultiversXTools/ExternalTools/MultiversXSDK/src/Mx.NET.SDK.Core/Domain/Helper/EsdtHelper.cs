using System;

namespace Mx.NET.SDK.Core.Domain.Helper
{
    public static class EsdtHelper
    {
        public static string GetTicker(this string identifier)
        {
            if (identifier.Contains("-"))
                return identifier.Substring(0, identifier.IndexOf('-'));
            else
                return identifier;
        }

        public static string GetCollection(this string identifier)
        {
            if (identifier.Contains("-"))
                return identifier.Substring(0, identifier.LastIndexOf('-'));
            else
                return identifier;
        }

        public static ulong GetNonce(this string identifier)
        {
            var nonce = identifier.Substring(identifier.LastIndexOf('-') + 1);
            return Convert.ToUInt64(nonce, 16);
        }
    }
}
