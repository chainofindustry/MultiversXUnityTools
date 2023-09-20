//#define DebugAPI
#if DebugAPI
using UnityEngine;
#endif

using Mx.NET.SDK.Provider;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain.Exceptions;
using System.Text;
using Mx.NET.SDK.Configuration;
using System.Collections.Generic;

namespace MultiversX.UnityTools
{
    public class ApiProviderUnity : ApiProvider
    {
        private string baseApiAddress;

        public ApiProviderUnity(ApiNetworkConfiguration configuration, Dictionary<string, string> extraRequestHeaders = null):base(configuration, extraRequestHeaders)
        {
            baseApiAddress = configuration.APIUri.AbsoluteUri;
        }

        #region IGenericProvider
        public override async Task<TR> Get<TR>(string requestUri)
        {
            requestUri = requestUri.StartsWith("/") ? requestUri[1..] : requestUri;
            string request = $"{baseApiAddress}{requestUri}";
#if DebugAPI
            Debug.Log("request: " + request);
#endif
            UnityWebRequest webRequest = UnityWebRequest.Get(request);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
#if DebugAPI
            Debug.Log("response: " + response);
#endif
            string content = webRequest.downloadHandler.text;
#if DebugAPI
            Debug.Log("content: " + content);
#endif
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    return JsonWrapper.Deserialize<TR>(content);
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
            }
        }

        public override async Task<TR> Post<TR>(string requestUri, object requestContent)
        {
            requestUri = requestUri.StartsWith("/") ? requestUri[1..] : requestUri;
            string jsonData = JsonWrapper.Serialize(requestContent);
#if DebugAPI
            Debug.Log("jsonData " + jsonData);
#endif
            var webRequest = new UnityWebRequest();
            webRequest.url = $"{baseApiAddress}{requestUri}";
#if DebugAPI
            Debug.Log("url " + webRequest.url);
#endif
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            UnityWebRequest.Result response = await webRequest.SendWebRequest();
#if DebugAPI
            Debug.Log("response: " + response);
#endif
            var content = webRequest.downloadHandler.text;
#if DebugAPI
            Debug.Log("content: " + content);
#endif
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonWrapper.Deserialize<TR>(content);
                    return result;
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
            }
        }
        #endregion
    }
}