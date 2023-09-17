using System.Text;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class ESDTIdentifierValue : BaseBinaryValue
    {
        public ESDTIdentifierValue(byte[] data, TypeValue type) : base(type)
        {
            Buffer = data;
            Value = Encoding.UTF8.GetString(data);
        }

        public string Value { get; }

        public byte[] Buffer { get; }

        public static ESDTIdentifierValue From(byte[] data)
        {
            return new ESDTIdentifierValue(data, TypeValue.TokenIdentifierValue);
        }

        // ReSharper disable once InconsistentNaming
        public static ESDTIdentifierValue EGLD()
        {
            return new ESDTIdentifierValue(new byte[0], TypeValue.TokenIdentifierValue);
        }

        public static ESDTIdentifierValue From(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return new ESDTIdentifierValue(bytes, TypeValue.TokenIdentifierValue);
        }

        public static bool operator ==(ESDTIdentifierValue value1, ESDTIdentifierValue value2) => value1.Equals(value2);
        public static bool operator !=(ESDTIdentifierValue value1, ESDTIdentifierValue value2) => !value1.Equals(value2);

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
                return Value.Equals(((ESDTIdentifierValue)obj).Value);
        }


        public bool IsEGLD()
        {
            if (Buffer.Length == 0)
                return true;

            if (Value == Constants.Constants.EGLD)
                return true;

            return false;
        }

        public override string ToString()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToJson()
        {
            return JsonWrapper.Serialize(Value);
        }
    }
}
