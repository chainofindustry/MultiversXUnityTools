using Erdcsharp.Provider.Dtos;
using System;
using System.Text;

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

        public static TransactionRequestDto ToSignedTransaction(this TransactionData tx, string signature)
        {
            TransactionRequestDto request = new TransactionRequestDto
            {
                Nonce = tx.nonce,
                Sender = tx.from,
                Receiver = tx.to,
                Value = tx.amount,
                GasPrice = long.Parse(tx.gasPrice),
                GasLimit = long.Parse(tx.gasLimit),
                Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(tx.data)),
                ChainID = tx.chainId,
                Version = tx.version,
                Signature = signature
            };
            return request;
        }
    }
}