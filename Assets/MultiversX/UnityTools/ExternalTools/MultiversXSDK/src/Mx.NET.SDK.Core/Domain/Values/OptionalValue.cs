using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class OptionalValue : BaseBinaryValue
    {
        public TypeValue InnerType { get; }
        public IBinaryType Value { get; }

        private OptionalValue(TypeValue type, TypeValue innerType = null, IBinaryType value = null) : base(type)
        {
            InnerType = innerType;
            Value = value;
        }

        public static OptionalValue NewMissing()
        {
            return new OptionalValue(TypeValue.OptionalValue());
        }

        public static OptionalValue NewProvided(IBinaryType value)
        {
            if (value is null)
                return NewMissing();
            return new OptionalValue(TypeValue.OptionalValue(value.Type), value.Type, value);
        }

        public bool IsSet()
        {
            return Value != null;
        }

        public override string ToString()
        {
            return IsSet() ? Value.ToString() : "";
        }

        public override T ToObject<T>()
        {
            return IsSet() ? JsonWrapper.Deserialize<T>(ToJson()) : default;
        }

        public override string ToJson()
        {
            return IsSet() ? Value.ToJson() : "{}";
        }
    }
}
