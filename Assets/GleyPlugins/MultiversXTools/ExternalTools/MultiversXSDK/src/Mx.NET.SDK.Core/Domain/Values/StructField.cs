namespace Mx.NET.SDK.Core.Domain.Values
{
    public class StructField
    {
        public IBinaryType Value { get; }
        public string Name { get; }

        public StructField(string name, IBinaryType value)
        {
            Value = value;
            Name = name;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
