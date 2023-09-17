using System.Linq;
using System.Numerics;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class NumericBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Numeric;

        private const int BytesSizeOfU32 = 4;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            if (type.HasFixedSize())
            {
                var length = type.SizeInBytes();
                var payload = data.Slice(0, length);
                var result = DecodeTopLevel(payload, type);
                return (result, length);
            }
            else
            {
                var sizeInBytes = data.ReadUInt32BE(0);
                var payload = data.Slice(BytesSizeOfU32, BytesSizeOfU32 + sizeInBytes);
                var bigNumber = Converter.ToBigInteger(payload, !type.HasSign(), isBigEndian: true);

                return (new NumericValue(type, bigNumber), BytesSizeOfU32 + payload.Length);
            }
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            if (data.Length == 0)
            {
                return new NumericValue(type, new BigInteger(0));
            }

            var bigNumber = Converter.ToBigInteger(data, !type.HasSign(), isBigEndian: true);
            return new NumericValue(type, bigNumber);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var numericValueObject = value.ValueOf<NumericValue>();
            if (value.Type.HasFixedSize())
            {
                var sizeInBytes = value.Type.SizeInBytes();
                var number = numericValueObject.Number;
                var fullArray = Enumerable.Repeat((byte)0x00, sizeInBytes).ToArray();
                if (number.IsZero)
                {
                    return fullArray;
                }

                var payload = Converter.FromBigInteger(number, !value.Type.HasSign(), true);
                var payloadLength = payload.Length;

                var data = fullArray.Slice(0, sizeInBytes - payloadLength).Concat(payload);
                return data.ToArray();
            }
            else
            {
                var payload = EncodeTopLevel(value);

                var lengthBuffer = new byte[4];
                lengthBuffer.WriteUInt32BE(payload.Length);

                var data = lengthBuffer.Concat(payload);
                return data.ToArray();
            }
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var numericValue = value.ValueOf<NumericValue>();
            if (numericValue.Number.IsZero)
            {
                return new byte[0];
            }

            var isUnsigned = !value.Type.HasSign();
            var buffer = Converter.FromBigInteger(numericValue.Number, isUnsigned, isBigEndian: true);

            return buffer;
        }
    }
}
