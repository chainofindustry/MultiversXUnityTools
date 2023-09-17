namespace Mx.NET.SDK.Core.Domain.Values
{
    public class EnumField
    {
        public IBinaryType Discriminant { get; }
        public string Name { get; }

        public EnumField(string name, IBinaryType value)
        {
            Discriminant = value;
            Name = name;
        }

        public override string ToString()
        {
            return Discriminant.ToString();
        }
    }
}
