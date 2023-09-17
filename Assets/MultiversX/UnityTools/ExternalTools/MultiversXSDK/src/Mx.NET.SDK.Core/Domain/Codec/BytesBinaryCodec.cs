using System.Linq;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class BytesBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Bytes;

        private const int BytesSizeOfU32 = 4;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var sizeInBytes = data.ReadUInt32BE(0);
            var payload = data.Slice(BytesSizeOfU32, BytesSizeOfU32 + sizeInBytes);

            return (new BytesValue(payload, type), BytesSizeOfU32 + payload.Length);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            return new BytesValue(data, type);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var bytesValueObject = value.ValueOf<BytesValue>();

            var lengthBuffer = new byte[4];
            lengthBuffer.WriteUInt32BE(bytesValueObject.GetLength());

            var data = lengthBuffer.Concat(bytesValueObject.Buffer);
            return data.ToArray();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var bytes = value.ValueOf<BytesValue>();
            return bytes.Buffer;
        }
    }
}
