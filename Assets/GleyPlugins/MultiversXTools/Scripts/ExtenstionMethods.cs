using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace MultiversXUnityTools
{
    public static class ExtenstionMethods
    {
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