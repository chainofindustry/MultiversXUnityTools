
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class UnityWebRequestExtension
{
    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
    {
        TaskCompletionSource<UnityWebRequest.Result> tsc = new();
        reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

        if (reqOp.isDone)
            tsc.TrySetResult(reqOp.webRequest.result);

        return tsc.Task.GetAwaiter();
    }
}
