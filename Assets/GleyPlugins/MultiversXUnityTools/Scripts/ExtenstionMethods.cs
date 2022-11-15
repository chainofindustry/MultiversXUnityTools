using Erdcsharp.Provider.Dtos;
using System;
using System.Text;

namespace MultiversXUnityTools
{
    public static class ExtenstionMethods
    {
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