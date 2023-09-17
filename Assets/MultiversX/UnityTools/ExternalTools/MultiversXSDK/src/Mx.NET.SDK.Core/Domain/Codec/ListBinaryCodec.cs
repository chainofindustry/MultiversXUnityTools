using System.Collections.Generic;
using System.Linq;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class ListBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;

        private const int BytesSizeOfU32 = 4;

        public ListBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public string Type => TypeValue.BinaryTypes.List;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var numItems = data.ReadUInt32BE(0);
            var result = new List<IBinaryType>();

            var originalBuffer = data;
            var offset = BytesSizeOfU32;

            data = originalBuffer.Slice(offset);

            for (int i = 0; i < numItems; i++)
            {
                var (value, bytesLength) = _binaryCodec.DecodeNested(data, type.InnerType);
                result.Add(value);
                offset += bytesLength;
                data = originalBuffer.Slice(offset);
            }

            var listValue = new ListValue(type, type.InnerType, result.ToArray());
            return (listValue, offset);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
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

            return ListValue.From(type, result.ToArray());
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var listValueObject = value.ValueOf<ListValue>();
            var buffers = new List<byte[]>();

            foreach (var listValue in listValueObject.Values)
            {
                var fieldBuffer = _binaryCodec.EncodeNested(listValue);
                buffers.Add(fieldBuffer);
            }

            var lengthBuffer = new byte[4];
            lengthBuffer.WriteUInt32BE(listValueObject.Values.Length);

            var data = lengthBuffer.Concat(buffers.SelectMany(b => b));
            return data.ToArray();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var listValue = value.ValueOf<ListValue>();
            var buffers = new List<byte[]>();

            foreach (var item in listValue.Values)
            {
                var buffer = _binaryCodec.EncodeNested(item);
                buffers.Add(buffer);
            }

            return buffers.SelectMany(b => b).ToArray();
        }
    }
}
