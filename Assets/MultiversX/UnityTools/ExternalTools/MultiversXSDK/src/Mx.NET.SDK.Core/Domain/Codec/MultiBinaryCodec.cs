using System.Collections.Generic;
using System.Linq;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class MultiBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;

        public MultiBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public string Type => TypeValue.BinaryTypes.Multi;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var result = new Dictionary<TypeValue, IBinaryType>();
            var originalBuffer = data;
            var offset = 0;

            foreach (var multiType in type.MultiTypes)
            {
                var (value, bytesLength) = _binaryCodec.DecodeNested(data, multiType);
                result.Add(multiType, value);
                offset += bytesLength;
                data = originalBuffer.Slice(offset);
            }

            var multiValue = new MultiValue(type, result);
            return (multiValue, offset);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var (value, _) = _binaryCodec.DecodeNested(data, type);
            return value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var multiValueObject = value.ValueOf<MultiValue>();
            var buffers = new List<byte[]>();

            foreach (var multiValue in multiValueObject.Values)
            {
                var fieldBuffer = _binaryCodec.EncodeNested(multiValue.Value);
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
