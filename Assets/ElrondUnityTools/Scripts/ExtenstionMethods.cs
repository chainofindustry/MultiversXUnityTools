using System;
namespace ElrondUnityTools
{
    public static class ExtenstionMethods
    {
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static string ToHex(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                    return ((byte)o).ToString("X");
                case TypeCode.SByte:
                    return ((sbyte)o).ToString("X");
                case TypeCode.UInt16:
                    return ((ushort)o).ToString("X");
                case TypeCode.UInt32:
                    return ((uint)o).ToString("X");
                case TypeCode.UInt64:
                    return ((ulong)o).ToString("X");
                case TypeCode.Int16:
                    return ((short)o).ToString("X");
                case TypeCode.Int32:
                    return ((int)o).ToString("X");
                case TypeCode.Int64:
                    return ((long)o).ToString("X");
                case TypeCode.Decimal:
                    return ((decimal)o).ToString("X");
                case TypeCode.Double:
                    return ((double)o).ToString("X");
                case TypeCode.Single:
                    return ((float)o).ToString("X");
            }
            return o.ToString();
        }
    }
}