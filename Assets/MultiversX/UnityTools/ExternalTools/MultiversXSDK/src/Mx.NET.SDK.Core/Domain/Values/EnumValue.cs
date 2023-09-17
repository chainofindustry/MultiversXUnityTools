using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class EnumValue : BaseBinaryValue
    {
        public EnumField Variant { get; }

        public EnumValue(TypeValue enumType, EnumField variant) : base(enumType)
        {
            Variant = variant;
        }

        public override string ToString()
        {
            return Variant.Name;
        }

        public override T ToObject<T>()
        {
            return JsonWrapper.Deserialize<T>(ToJson());
        }

        public override string ToJson()
        {
            return JsonUnqtWrapper.Serialize(Variant.Discriminant.ToJson());
        }
    }
}
