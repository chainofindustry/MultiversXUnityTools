using Newtonsoft.Json;

namespace Mx.NET.SDK.Core.Domain.Abi
{
    public class Abi
    {
        public class Input
        {
            public string Name { get; set; }
            public string Type { get; set; }

            [JsonProperty("multi_arg")]
            public bool MultiArg { get; set; }
        }

        public class CustomTypes
        {
            public string Type { get; set; }
            public Field[] Fields { get; set; }
            public Variant[] Variants { get; set; }
        }

        public class Field
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class Variant
        {
            public string Name { get; set; }
            public int Discriminant { get; set; }
        }

        public class Endpoint
        {
            public string Name { get; set; }
            public Input[] Inputs { get; set; }
            public Output[] Outputs { get; set; }
            public string[] PayableInTokens { get; set; }
        }

        public class Event
        {
            public string Identifier { get; set; }
            public Input[] Inputs { get; set; }
        }

        public class Output
        {
            public string Type { get; set; }

            [JsonProperty("multi_result")]
            public bool MultiResult { get; set; }
        }
    }
}
