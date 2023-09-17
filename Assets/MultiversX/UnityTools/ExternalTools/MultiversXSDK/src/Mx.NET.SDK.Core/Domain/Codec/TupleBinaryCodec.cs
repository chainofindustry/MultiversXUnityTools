using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using System.Collections.Generic;
using System.Linq;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class TupleBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;

        public TupleBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public string Type => TypeValue.BinaryTypes.Tuple;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var result = new Dictionary<TypeValue, IBinaryType>();
            var originalBuffer = data;
            var offset = 0;

            foreach (var tupleType in type.MultiTypes)
            {
                var (value, bytesLength) = _binaryCodec.DecodeNested(data, tupleType);
                result.Add(tupleType, value);
                offset += bytesLength;
                data = originalBuffer.Slice(offset);
            }

            var tupleValue = new TupleValue(type, result);
            return (tupleValue, offset);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var (value, _) = _binaryCodec.DecodeNested(data, type);
            return value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var tupleValueObject = value.ValueOf<TupleValue>();
            var buffers = new List<byte[]>();

            foreach (var tupleValue in tupleValueObject.Values)
            {
                var fieldBuffer = _binaryCodec.EncodeNested(tupleValue.Value);
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
