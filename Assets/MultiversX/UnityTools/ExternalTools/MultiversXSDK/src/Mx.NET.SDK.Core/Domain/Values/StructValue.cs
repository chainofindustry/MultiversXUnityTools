using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mx.NET.SDK.Core.Domain.Exceptions;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class StructValue : BaseBinaryValue
    {
        public StructField[] Fields { get; }

        public StructValue(TypeValue structType, StructField[] fields) : base(structType)
        {
            Fields = fields;
            CheckTyping();
        }

        public StructField GetStructField(string name)
        {
            var field = Fields.SingleOrDefault(f => f.Name == name);
            return field;
        }

        private void CheckTyping()
        {
            var definitions = Type.GetFieldDefinitions();
            if (Fields.Length != definitions.Length)
            {
                throw new BinaryCodecException("fields length vs. field definitions length");
            }

            for (var i = 0; i < Fields.Length; i++)
            {
                var field = Fields[i];
                var definition = definitions[i];
                var fieldType = field.Value.Type;

                if (fieldType.RustType != definition.Type.RustType)
                    throw new BinaryCodecException("field rustType vs. field definitions rustType");
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Type.Name);
            foreach (var structField in Fields)
            {
                builder.AppendLine($"{structField.Name}:{structField.Value}");
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
            foreach (var field in Fields)
            {
                dict.Add(field.Name, field.Value.ToJson());
            }

            return JsonUnqtWrapper.Serialize(dict);
        }
    }
}
