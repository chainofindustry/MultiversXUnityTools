using Erdcsharp.Provider.Dtos;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace MultiversXUnityTools
{
    public static class ExtenstionMethods
    {
        /// <summary>
        /// Apply signature to transaction
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Extension method to make Unity web request run as a task(required by ElrondSDK)
        /// </summary>
        /// <param name="reqOp"></param>
        /// <returns></returns>
        public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
        {
            TaskCompletionSource<UnityWebRequest.Result> tsc = new();
            reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

            if (reqOp.isDone)
                tsc.TrySetResult(reqOp.webRequest.result);

            return tsc.Task.GetAwaiter();
        }
    }
}