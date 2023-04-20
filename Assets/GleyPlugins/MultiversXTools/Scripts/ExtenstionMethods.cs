using Mx.NET.SDK.Provider.Dtos.API.Transactions;
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
                Sender = tx.sender,
                Receiver = tx.receiver,
                Value = tx.value,
                GasPrice = tx.gasPrice,
                GasLimit = tx.gasLimit,
                Data = tx.data,
                ChainID = tx.chainID,
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