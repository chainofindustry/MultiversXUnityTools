using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Abi
{
    public class EventDefinition
    {
        public string Identifier { get; }
        public FieldDefinition[] Input { get; }

        public EventDefinition(string identifier, FieldDefinition[] input)
        {
            Identifier = identifier;
            Input = input;
        }
    }
}
