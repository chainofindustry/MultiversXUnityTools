using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class TupleValue : BaseBinaryValue
    {
        public Dictionary<TypeValue, IBinaryType> Values { get; }

        public TupleValue(TypeValue type, Dictionary<TypeValue, IBinaryType> values) : base(type)
        {
            Values = values;
        }

        public static TupleValue From(params IBinaryType[] values)
        {
            var t = values.Select(s => s.Type).ToArray();
            return new TupleValue(TypeValue.TupleValue(t), values.ToDictionary(s => s.Type, d => d));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Type.Name);
            foreach (var value in Values)
            {
                builder.AppendLine($"{value.Key}:{value}");
            }

            return builder.ToString();
        }

        public override T ToObject<T>()
        {
            return JsonWrapper.Deserialize<T>(ToJson());
        }

        public override string ToJson()
        {
            var dict = new Dictionary<string, object>();
            for (var i = 0; i < Values.Count; i++)
            {
                var value = Values.ToArray()[i];
                dict.Add($"item{i + 1}", value.Value.ToJson());
            }

            return JsonUnqtWrapper.Serialize(dict);
        }
    }
}
