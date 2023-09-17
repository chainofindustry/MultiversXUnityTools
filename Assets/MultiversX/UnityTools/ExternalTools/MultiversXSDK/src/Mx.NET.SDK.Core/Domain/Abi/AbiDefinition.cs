using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mx.NET.SDK.Core.Domain.Values;

namespace Mx.NET.SDK.Core.Domain.Abi
{
    public class AbiDefinition
    {
        public string Name { get; set; }
        public Abi.Endpoint[] Endpoints { get; set; }
        public Abi.Event[] Events { get; set; }
        public Dictionary<string, Abi.CustomTypes> Types { get; set; }

        public EndpointDefinition GetEndpointDefinition(string endpoint)
        {
            var data = Endpoints.ToList().SingleOrDefault(s => s.Name == endpoint) ?? throw new Exception("Endpoint is not defined in ABI");

            var inputs = data.Inputs is null ?
                new List<FieldDefinition>() :
                data.Inputs.Select(i => new FieldDefinition(i.Name, "", GetTypeValue(i.Type))).ToList();
            var outputs = data.Outputs is null ?
                new List<FieldDefinition>() :
                data.Outputs.Select(i => new FieldDefinition("", "", GetTypeValue(i.Type))).ToList();

            return new EndpointDefinition(endpoint, inputs.ToArray(), outputs.ToArray());
        }

        public EventDefinition GetEventDefinition(string identifier)
        {
            var data = Events.ToList().SingleOrDefault(s => s.Identifier == identifier) ?? throw new Exception("Event is not defined in ABI");

            var inputs = data.Inputs is null ?
                new List<FieldDefinition>() :
                data.Inputs.Select(i => new FieldDefinition(i.Name, "", GetTypeValue(i.Type))).ToList();

            return new EventDefinition(identifier, inputs.ToArray());
        }

        private TypeValue GetTypeValue(string type)
        {
            var pattern = new Regex("^(.*?)<(.*)>$");
            if (pattern.IsMatch(type))
            {
                var parentType = pattern.Match(type).Groups[1].Value;
                var innerType = pattern.Match(type).Groups[2].Value;

                var innerTypes = pattern.IsMatch(innerType) ? new[] { innerType } : innerType.Split(',').Where(s => !string.IsNullOrEmpty(s));
                var innerTypeValues = innerTypes.Select(GetTypeValue).ToArray();
                var typeFromLearnedTypes = TypeValue.FromLearnedType(parentType, innerTypeValues);
                if (typeFromLearnedTypes != null)
                    return typeFromLearnedTypes;
            }

            var typeFromBaseRustType = TypeValue.FromRustType(type);
            if (typeFromBaseRustType != null)
                return typeFromBaseRustType;

            if (Types.Keys.Contains(type))
            {
                var typeFromStruct = Types[type];
                if (typeFromStruct.Type == "enum")
                {
                    return TypeValue.EnumValue(typeFromStruct.Type,
                                               typeFromStruct.Variants?
                                                    .ToList()
                                                    .Select(c => new FieldDefinition(c.Name, "", GetTypeValue(TypeValue.FromRustType("Enum").RustType)))
                                                    .ToArray());
                }
                else if (typeFromStruct.Type == "struct")
                {
                    return TypeValue.StructValue(typeFromStruct.Type,
                                                 typeFromStruct.Fields?
                                                    .ToList()
                                                    .Select(c => new FieldDefinition(c.Name, "", GetTypeValue(c.Type)))
                                                    .ToArray());

                }
            }

            return null;
        }

        public static AbiDefinition FromJson(string json)
        {
            return Helper.JsonWrapper.Deserialize<AbiDefinition>(json);
        }

        public static AbiDefinition FromFilePath(string jsonFilePath)
        {
            var fileBytes = File.ReadAllBytes(jsonFilePath);
            var json = Encoding.UTF8.GetString(fileBytes);
            return FromJson(json);
        }
    }
}
