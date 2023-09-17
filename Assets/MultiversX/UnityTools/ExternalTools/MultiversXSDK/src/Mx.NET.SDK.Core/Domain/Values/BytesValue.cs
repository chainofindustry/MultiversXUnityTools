using System.Text;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class BytesValue : BaseBinaryValue
    {
        public BytesValue(byte[] data, TypeValue type)
            : base(type)
        {
            Buffer = data;
        }

        public int GetLength()
        {
            return Buffer.Length;
        }

        public static BytesValue FromUtf8(string utf8String)
        {
            return new BytesValue(Encoding.UTF8.GetBytes(utf8String), TypeValue.BytesValue);
        }

        public static BytesValue FromHex(string hexString)
        {
            return new BytesValue(Converter.FromHexString(hexString), TypeValue.BytesValue);
        }

        public static BytesValue FromBuffer(byte[] data)
        {
            return new BytesValue(data, TypeValue.BytesValue);
        }

        public byte[] Buffer { get; }

        public override string ToString()
        {
            return Converter.ToHexString(Buffer);
        }
    }
}
