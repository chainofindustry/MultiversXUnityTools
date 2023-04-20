using System;
using System.Text;

namespace Mx.NET.SDK.Core.Domain.Helper
{
    public static class DataCoder
    {
        public static string DecodeData(string encodedData)
        {
            if (encodedData is null) return null;

            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedData));
        }

        public static string EncodeData(string data)
        {
            if (data is null) return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }
}
