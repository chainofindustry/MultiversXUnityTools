using Erdcsharp.Provider.Dtos;
using System;
using System.Text;
using System.Numerics;
using Erdcsharp.Domain;

namespace ElrondUnityTools
{
    public static class ExtenstionMethods
    {
        public static string ToHex(this TokenAmount nr)
        {
            string result = nr.Value.ToString("X");
            if (result.Length % 2 == 1)
            {
                result = "0" + result;
            }
            return result;
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