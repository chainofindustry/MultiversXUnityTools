using Newtonsoft.Json;

namespace Mx.NET.SDK.Core.Domain.Values
{
    public class TypeValue
    {
        public string Name { get; private set; }

        public string BinaryType { get; }
        public string RustType { get; }
        public TypeValue InnerType { get; }
        public TypeValue[] MultiTypes { get; }
        public int? Length { get; }

        private readonly int? _sizeInBytes;
        private readonly bool? _withSign;
        private readonly FieldDefinition[] _fieldDefinitions;

        [JsonConstructor]
        public TypeValue(string binaryType, string rustType, int? sizeInBytes = null, bool? withSign = null)
        {
            BinaryType = binaryType;
            RustType = rustType;
            _sizeInBytes = sizeInBytes;
            _withSign = withSign;
        }

        public TypeValue(string binaryType, string rustType, FieldDefinition[] fieldDefinitions)
        {
            BinaryType = binaryType;
            RustType = rustType;
            _fieldDefinitions = fieldDefinitions;
        }

        public TypeValue(string binaryType, TypeValue innerType = null, int? length = null)
        {
            BinaryType = binaryType;
            InnerType = innerType;
            Length = length;
        }

        public TypeValue(string binaryType, TypeValue[] multiTypes)
        {
            BinaryType = binaryType;
            MultiTypes = multiTypes;
        }

        public void SetName(string name) => Name = name;

        public int SizeInBytes()
        {
            return _sizeInBytes ?? U32TypeValue.SizeInBytes();
        }

        public bool HasSign()
        {
            return _withSign ?? false;
        }

        public bool HasFixedSize()
        {
            return _sizeInBytes.HasValue;
        }

        public bool HasArbitrarySize()
        {
            return !HasFixedSize();
        }

        public bool IsCounted()
        {
            return Length == 1 && InnerType.BinaryType == BinaryTypes.Variadic;
        }

        public static class BinaryTypes
        {
            public const string Boolean = nameof(Boolean);
            public const string Address = nameof(Address);
            public const string Numeric = nameof(Numeric);
            public const string Struct = nameof(Struct);
            public const string Bytes = nameof(Bytes);
            public const string TokenIdentifier = nameof(TokenIdentifier);
            public const string Option = nameof(Option);
            public const string Optional = nameof(Optional);
            public const string Multi = nameof(Multi);
            public const string Tuple = nameof(Tuple);
            public const string Variadic = nameof(Variadic);
            public const string List = nameof(List);
            public const string Array = nameof(Array);
            public const string Enum = nameof(Enum);
        }

        public static class LearnedTypes
        {
            public const string Option = "Option";
            public const string List = "List";
            public const string VarArgs = "VarArgs";
            public const string MultiResultVec = "MultiResultVec";
            public const string Variadic = "variadic";
            public const string CountedVariadic = "counted-variadic";
            public const string OptionalArg = "OptionalArg";
            public const string Optional = "optional";
            public const string OptionalResult = "OptionalResult";
            public const string Multi = "multi";
            public const string MultiArg = "MultiArg";
            public const string MultiResult = "MultiResult";
            public const string Tuple = "tuple";
            public const string Tuple2 = "tuple2";
            public const string Tuple3 = "tuple3";
            public const string Tuple4 = "tuple4";
            public const string Tuple5 = "tuple5";
            public const string Tuple6 = "tuple6";
            public const string Tuple7 = "tuple7";
            public const string Tuple8 = "tuple8";
            public const string Array = "Array";
            public const string Array2 = "array2";
            public const string Array8 = "array8";
            public const string Array16 = "array16";
            public const string Array20 = "array20";
            public const string Array32 = "array32";
            public const string Array46 = "array46";
            public const string Array64 = "array64";
            public const string Array128 = "array128";
            public const string Array256 = "array256";
        }

        public static class RustTypes
        {
            public const string U8 = "u8";
            public const string U16 = "u16";
            public const string U32 = "u32";
            public const string U64 = "u64";
            public const string BigUint = "BigUint";

            public const string I8 = "i8";
            public const string I16 = "i16";
            public const string I32 = "i32";
            public const string I64 = "i64";
            public const string Bigint = "BigInt";

            public const string Bool = "bool";
            public const string Bytes = "bytes";
            public const string Address = "Address";
            public const string H256 = "H256";
            public const string TokenIdentifier = "TokenIdentifier";
            public const string EgldOrEsdtTokenIdentifier = "EgldOrEsdtTokenIdentifier";
            public const string Enum = "Enum";
        }

        public static TypeValue U8TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U8, 1, false);
        public static TypeValue I8TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I8, 1, true);

        public static TypeValue U16TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U16, 2, false);
        public static TypeValue I16TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I16, 2, true);

        public static TypeValue U32TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U32, 4, false);
        public static TypeValue I32TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I32, 4, true);

        public static TypeValue U64TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U64, 8, false);
        public static TypeValue I64TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I64, 8, true);

        public static TypeValue BigUintTypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.BigUint, null, false);
        public static TypeValue BigIntTypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.Bigint, null, true);

        public static TypeValue BooleanValue => new TypeValue(BinaryTypes.Boolean, RustTypes.Bool);
        public static TypeValue AddressValue => new TypeValue(BinaryTypes.Address, RustTypes.Address);

        public static TypeValue TokenIdentifierValue => new TypeValue(BinaryTypes.TokenIdentifier, RustTypes.TokenIdentifier);
        public static TypeValue ScResult => new TypeValue(BinaryTypes.Bytes, RustTypes.Bytes);

        public static TypeValue BytesValue => new TypeValue(BinaryTypes.Bytes, RustTypes.Bytes);
        public static TypeValue H256Value => new TypeValue(BinaryTypes.Bytes, RustTypes.H256);

        public static TypeValue OptionValue(TypeValue innerType = null) => new TypeValue(BinaryTypes.Option, innerType);
        public static TypeValue OptionalValue(TypeValue innerType = null) => new TypeValue(BinaryTypes.Optional, innerType);
        public static TypeValue MultiValue(TypeValue[] multiTypes) => new TypeValue(BinaryTypes.Multi, multiTypes);
        public static TypeValue TupleValue(TypeValue[] tupleTypes) => new TypeValue(BinaryTypes.Tuple, tupleTypes);
        public static TypeValue VariadicValue(TypeValue innerType, bool isCounted = false) => new TypeValue(BinaryTypes.Variadic, innerType, isCounted == false ? 0 : 1);
        public static TypeValue ListValue(TypeValue innerType) => new TypeValue(BinaryTypes.List, innerType);
        public static TypeValue ArrayValue(TypeValue innerType, int length) => new TypeValue(BinaryTypes.Array, innerType, length);

        public static TypeValue StructValue(string name, FieldDefinition[] fieldDefinitions) =>
            new TypeValue(BinaryTypes.Struct, name, fieldDefinitions);
        public static TypeValue EnumValue(string name, FieldDefinition[] fieldDefinitions) =>
            new TypeValue(BinaryTypes.Enum, name, fieldDefinitions);

        public static TypeValue FromLearnedType(string learnedType, TypeValue[] types)
        {
            switch (learnedType)
            {
                case LearnedTypes.Option:
                    return OptionValue(types[0]);

                case LearnedTypes.List:
                    return ListValue(types[0]);

                case LearnedTypes.VarArgs:
                case LearnedTypes.MultiResultVec:
                case LearnedTypes.Variadic:
                    return VariadicValue(types[0]);
                case LearnedTypes.CountedVariadic:
                    return VariadicValue(types[0], true);

                case LearnedTypes.OptionalArg:
                case LearnedTypes.Optional:
                case LearnedTypes.OptionalResult:
                    return OptionalValue(types[0]);

                case LearnedTypes.Multi:
                case LearnedTypes.MultiArg:
                case LearnedTypes.MultiResult:
                    return MultiValue(types);

                case LearnedTypes.Tuple:
                case LearnedTypes.Tuple2:
                case LearnedTypes.Tuple3:
                case LearnedTypes.Tuple4:
                case LearnedTypes.Tuple5:
                case LearnedTypes.Tuple6:
                case LearnedTypes.Tuple7:
                case LearnedTypes.Tuple8:
                    return TupleValue(types);

                case LearnedTypes.Array2:
                    return ArrayValue(types[0], 2);
                case LearnedTypes.Array8:
                    return ArrayValue(types[0], 8);
                case LearnedTypes.Array16:
                    return ArrayValue(types[0], 16);
                case LearnedTypes.Array20:
                    return ArrayValue(types[0], 20);
                case LearnedTypes.Array32:
                    return ArrayValue(types[0], 32);
                case LearnedTypes.Array46:
                    return ArrayValue(types[0], 46);
                case LearnedTypes.Array64:
                    return ArrayValue(types[0], 64);
                case LearnedTypes.Array128:
                    return ArrayValue(types[0], 128);
                case LearnedTypes.Array256:
                    return ArrayValue(types[0], 256);

                default:
                    return null;
            }
        }

        public static TypeValue FromRustType(string rustType)
        {
            switch (rustType)
            {
                case RustTypes.U8:
                    return U8TypeValue;
                case RustTypes.U16:
                    return U16TypeValue;
                case RustTypes.U32:
                    return U32TypeValue;
                case RustTypes.U64:
                    return U64TypeValue;
                case RustTypes.BigUint:
                    return BigUintTypeValue;

                case RustTypes.I8:
                    return I8TypeValue;
                case RustTypes.I16:
                    return I16TypeValue;
                case RustTypes.I32:
                    return I32TypeValue;
                case RustTypes.I64:
                    return I64TypeValue;
                case RustTypes.Bigint:
                    return BigIntTypeValue;

                case RustTypes.Bool:
                    return BooleanValue;
                case RustTypes.Bytes:
                    return BytesValue;
                case RustTypes.Address:
                    return AddressValue;
                case RustTypes.TokenIdentifier:
                    return TokenIdentifierValue;
                case RustTypes.EgldOrEsdtTokenIdentifier:
                    return TokenIdentifierValue;
                case RustTypes.Enum:
                    return U8TypeValue;

                default:
                    return null;
            }
        }

        public FieldDefinition[] GetFieldDefinitions()
        {
            return _fieldDefinitions;
        }
    }
}
