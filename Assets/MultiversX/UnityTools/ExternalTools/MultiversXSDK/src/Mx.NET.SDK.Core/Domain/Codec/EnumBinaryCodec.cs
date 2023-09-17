using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mx.NET.SDK.Core.Domain.Codec
{
	public class EnumBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;
        public string Type => TypeValue.BinaryTypes.Enum;

        public EnumBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var fieldDefinitions = type.GetFieldDefinitions();
            var fields = new List<EnumField>();
            var originalBuffer = data;

            var (value, bytesLength) = _binaryCodec.DecodeNested(data, fieldDefinitions[0].Type);

            var index = int.Parse(value.ToString());
            var enumValue = new EnumValue(type, new EnumField(fieldDefinitions[index].Name, value));
            return (enumValue, 1);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var (value, _) = DecodeNested(data, type);
            return value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var enumValue = value.ValueOf<EnumValue>();
            return Encoding.ASCII.GetBytes(enumValue.Variant.Discriminant.ToString());
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            return EncodeNested(value);
        }
    }
}
