using System.Collections.Generic;
using System.Linq;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class ArrayBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;

        public ArrayBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public string Type => TypeValue.BinaryTypes.Array;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var result = new List<IBinaryType>();
            var originalBuffer = data;
            var offset = 0;

            for (int i = 0; i < type.Length; i++)
            {
                var (value, bytesLength) = _binaryCodec.DecodeNested(data, type.InnerType);
                result.Add(value); 
                offset += bytesLength;
                data = originalBuffer.Slice(offset);
            }

            var arrayValue = new ArrayValue(type, type.InnerType, result.ToArray());
            return (arrayValue, offset);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var (value, _) = DecodeNested(data, type);
            return value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var arrayValueObject = value.ValueOf<ArrayValue>();
            var buffers = new List<byte[]>();

            foreach (var arrayValue in arrayValueObject.Values)
            {
                var fieldBuffer = _binaryCodec.EncodeNested(arrayValue);
                buffers.Add(fieldBuffer);
            }

            var data = buffers.SelectMany(s => s);
            return data.ToArray();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            return EncodeNested(value);
        }
    }
}
