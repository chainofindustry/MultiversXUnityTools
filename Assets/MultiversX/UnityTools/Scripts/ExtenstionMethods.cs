using Mx.NET.SDK.Domain.Data.Transactions;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace MultiversX.UnityTools
{
    public static class ExtenstionMethods
    {
        /// <summary>
        /// Extension method to make Unity web request run as a task(required by MXSDK)
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

        public static string GetLogs(this Transaction tx)
        {
            string logs = "";
            if (tx.Logs != null)
            {
                if (tx.Logs.Events != null)
                {
                    for (int j = 0; j < tx.Logs.Events.Length; j++)
                    {
                        logs += " " + tx.Logs.Events[j].Identifier;
                        if (tx.Logs.Events[j].Topics != null)
                        {
                            if (tx.Logs.Events[j].Topics.Length > 0)
                            {
                                for (int k = 1; k < tx.Logs.Events[j].Topics.Length; k++)
                                {
                                    logs += $" {Encoding.UTF8.GetString(Convert.FromBase64String(tx.Logs.Events[j].Topics[k]))}";
                                }
                            }
                        }
                    }
                }
            }
            return logs;
        }
    }
}