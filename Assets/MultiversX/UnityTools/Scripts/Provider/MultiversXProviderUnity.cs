//#define DebugAPI
using Microsoft.IdentityModel.Tokens;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Provider.Dtos.API.Account;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Provider.Dtos.Gateway;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Dtos.Gateway.Query;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MultiversX.UnityTools
{
    /// <summary>
    /// You can implement your own version of provider
    /// Used to download data from blockchain API
    /// </summary>
    public class MultiversXProviderUnity : IMultiversXUnityApi
    {
        string baseApiAddress;
        string baseGatewayAddress;

        public MultiversXProviderUnity(MultiversxNetworkConfiguration config)
        {
            baseApiAddress = config.APIUri.AbsoluteUri;
            baseGatewayAddress = config.GatewayUri.AbsoluteUri;
        }

        #region IApiProvider
        public async Task<TR> Get<TR>(string requestUri)
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
            var content = webRequest.downloadHandler.text;
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


        public async Task<TR> Post<TR>(string requestUri, object requestContent)
        {
            requestUri = requestUri.StartsWith("/") ? requestUri[1..] : requestUri;
            string jsonData = JsonWrapper.Serialize(requestContent);
            var webRequest = new UnityWebRequest();
            webRequest.url = $"{baseApiAddress}{requestUri}";
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
                    var result = JsonWrapper.Deserialize<TR>(content);
                    return result;
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
            }
        }


        public async Task<AccountDto> GetAccount(string address)
        {
            return await Get<AccountDto>($"accounts/{address}");
        }
        #endregion


        #region IGatewayProvider
        public async Task<TR> GetGW<TR>(string requestUri)
        {
            requestUri = requestUri.StartsWith("/") ? requestUri[1..] : requestUri;
            string request = $"{baseGatewayAddress}{requestUri}";
#if DebugAPI
            Debug.Log("request " + request);
#endif
            UnityWebRequest webRequest = UnityWebRequest.Get(request);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
#if DebugAPI
            Debug.Log("response " + response);
#endif
            var content = webRequest.downloadHandler.text;
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
                    if (content.IsNullOrEmpty())
                    {
                        throw new APIException($"{request} {response}");
                    }
                    throw new APIException(content);
            }
        }


        public async Task<TR> PostGW<TR>(string requestUri, object requestContent)
        {
            requestUri = requestUri.StartsWith("/") ? requestUri[1..] : requestUri;
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


        public async Task<GatewayNetworkConfigDataDto> GetGatewayNetworkConfig()
        {
            return await GetGW<GatewayNetworkConfigDataDto>("network/config");
        }


        public async Task<QueryVmResponseDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            return await PostGW<QueryVmResponseDto>("vm-values/query", queryVmRequestDto);
        }


        public async Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            return await PostGW<TransactionResponseDto>("transaction/send", transactionRequestDto);
        }


        public async Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequestDto)
        {
            return await PostGW<MultipleTransactionsResponseDto>("transaction/send-multiple", transactionsRequestDto);
        }
        #endregion


        #region ITransactionsProvider
        public async Task<TransactionDto> GetTransaction(string txHash)
        {
            return await GetTransaction<TransactionDto>(txHash);
        }


        public async Task<Transaction> GetTransaction<Transaction>(string txHash)
        {
            return await Get<Transaction>($"transactions/{txHash}");
        }


        public async Task<string> GetTransactionsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"transactions/count{args}");
        }


        public async Task<TransactionDto[]> GetTransactions(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetTransactions<TransactionDto>(size, from, parameters);
        }


        public async Task<Transaction[]> GetTransactions<Transaction>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<Transaction[]>($"transactions?from={from}&size={size}{args}");
        }
        #endregion


        #region IMultiversXUnityApi 
        public async Task<T> GetWalletNfts<T>(string address)
        {
            string url = $"accounts/{address}/nfts/count";
            int totalNfts = await Get<int>(url);
            url = $"accounts/{address}/nfts?from={0}&size={totalNfts}";
            return await Get<T>(url);
        }

        public async Task<T> GetWalletTokens<T>(string address)
        {
            string url = $"accounts/{address}/tokens/count";
            int totalTokens = await Get<int>(url);
            url = $"accounts/{address}/tokens?from={0}&size={totalTokens}";
            return await Get<T>(url);
        }
        #endregion
    }
}