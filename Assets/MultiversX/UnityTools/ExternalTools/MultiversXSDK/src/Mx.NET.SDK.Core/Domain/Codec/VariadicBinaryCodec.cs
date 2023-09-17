using System.Collections.Generic;
using System.Linq;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class VariadicBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;

        public VariadicBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public string Type => TypeValue.BinaryTypes.Variadic;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var result = new List<IBinaryType>();
            var originalBuffer = data;
            var offset = 0;

            while (data.Length > 0)
            {
                var (value, bytesLength) = _binaryCodec.DecodeNested(data, type.InnerType);
                result.Add(value);
                offset += bytesLength;
                data = originalBuffer.Slice(offset);
            }

            var variadicValue = new VariadicValue(type, type.InnerType, result.ToArray());
            return (variadicValue, offset);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var (value, _) = _binaryCodec.DecodeNested(data, type);
            return value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var variadicValueObject = value.ValueOf<VariadicValue>();
            var buffers = new List<byte[]>();

            foreach (var variadicValue in variadicValueObject.Values)
            {
                var fieldBuffer = _binaryCodec.EncodeNested(variadicValue);
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
