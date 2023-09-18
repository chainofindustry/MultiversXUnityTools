using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.Provider.Dtos.Gateway;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace MultiversX.UnityTools
{
    public class GatewayProviderUnity : GatewayProvider
    {
        string baseGatewayAddress;

        public GatewayProviderUnity(GatewayNetworkConfiguration configuration, Dictionary<string, string> extraRequestHeaders = null):base (configuration, extraRequestHeaders)
        {
            baseGatewayAddress = configuration.GatewayUri.AbsoluteUri;
        }


        #region IGenericProvider
        public override async Task<TR> Get<TR>(string requestUri)
        {
            requestUri = requestUri.StartsWith("/") ? requestUri.Substring(1) : requestUri;
            string request = $"{baseGatewayAddress}{requestUri}";
#if DebugAPI
            Debug.Log("request " + request);
#endif
            UnityWebRequest webRequest = UnityWebRequest.Get(request);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
#if DebugAPI
            Debug.Log("response " + response);
#endif
            string content = webRequest.downloadHandler.text;
#if DebugAPI
            Debug.Log("content: " + content);
#endif
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonWrapper.Deserialize<GatewayResponseDto<TR>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    if (string.IsNullOrEmpty(content))
                    {
                        throw new APIException($"{request} {response}");
                    }
                    throw new APIException(content);
            }
        }


        public override async Task<TR> Post<TR>(string requestUri, object requestContent)
        {
            requestUri = requestUri.StartsWith("/") ? requestUri.Substring(1) : requestUri;
            string jsonData = JsonWrapper.Serialize(requestContent);
            var webRequest = new UnityWebRequest();
            webRequest.url = $"{baseGatewayAddress}{requestUri}";
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonWrapper.Deserialize<GatewayResponseDto<TR>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new APIException(content);
            }
        }
        #endregion
    }
}