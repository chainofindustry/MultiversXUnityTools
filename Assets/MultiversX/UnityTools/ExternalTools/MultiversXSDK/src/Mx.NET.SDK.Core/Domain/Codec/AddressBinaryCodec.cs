using System.Linq;
using Mx.NET.SDK.Core.Domain.Exceptions;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Codec
{
    public class AddressBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Address;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            // We don't check the size of the buffer, we just read 32 bytes.
            var addressBytes = data.Take(32).ToArray();
            var value = Address.FromBytes(addressBytes);
            return (value, addressBytes.Length);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var (value, _) = DecodeNested(data, type);
            return value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var address = Get(value);
            return address.PublicKey();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var address = Get(value);
            return address.PublicKey();
        }

        private static Address Get(IBinaryType value)
        {
            if (value is Address address)
            {
                return address;
            }

            throw new WrongBinaryValueCodecException();
        }
    }
}
