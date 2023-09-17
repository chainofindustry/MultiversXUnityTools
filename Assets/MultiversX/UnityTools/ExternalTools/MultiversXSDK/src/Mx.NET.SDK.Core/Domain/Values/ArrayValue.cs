using Mx.NET.SDK.Core.Domain.Helper;
using System.Linq;
using System.Text;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class ArrayValue : BaseBinaryValue
    {
        public TypeValue InnerType { get; }
        public IBinaryType[] Values { get; }

        public ArrayValue(TypeValue type, TypeValue innerType, IBinaryType[] values) : base(type)
        {
            InnerType = innerType;
            Values = values;
        }

        public static ArrayValue From(TypeValue type, params IBinaryType[] values)
        {
            return new ArrayValue(TypeValue.ArrayValue(type, values.Length), type.InnerType, values);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Type.Name);
            foreach (var value in Values)
            {
                builder.AppendLine(value.ToString());
            }

            return builder.ToString();
        }

        public override T ToObject<T>()
        {
            return JsonWrapper.Deserialize<T>(ToJson());
        }

        public override string ToJson()
        {
            var json = Values.Select(v => v.ToJson()).ToArray();
            return $"[{string.Join(",", json)}]";
        }
    }
}
