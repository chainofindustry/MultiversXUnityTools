using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using UnityEngine;
using UnityEngine.Networking;

namespace MultiversXUnityTools
{
    /// <summary>
    /// You can implement your own version of provider
    /// Used to download data from API
    /// </summary>
    public class ElrondProviderUnity : IMultiversXApiProvider
    {
        API selectedAPI;
        public ElrondProviderUnity(API api)
        {
            selectedAPI = api;
        }

        #region IElrondProvider
        public async Task<ConfigDataDto> GetNetworkConfig()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(selectedAPI.GetEndpoint(EndpointNames.GetNetworkConfig));
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<ConfigDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<AccountDto> GetAccount(string address)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetAccount).Replace("{address}", address);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<AccountDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data.Account;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<EsdtItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetEsdtNftToken).Replace("{address}", address).Replace("{tokenIdentifier}", tokenIdentifier).Replace("{tokenId}", tokenId.ToString());
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtItemDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetEsdtToken).Replace("{address}", address).Replace("{tokenIdentifier}", tokenIdentifier);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtTokenData>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<EsdtTokenDataDto> GetEsdtTokens(string address)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetEsdtTokens).Replace("{address}", address);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtTokenDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }



        public async Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(transactionRequestDto);

            var webRequest = new UnityWebRequest();
            webRequest.url = selectedAPI.GetEndpoint(EndpointNames.GetTransactionCost);
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(raw));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<TransactionCostDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<TransactionResponse> GetTransactionDetails(string txHash)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetTransactionDetail).Replace("{txHash}", txHash);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log(content);
                    var result = JsonSerializerWrapper.Deserialize<TransactionResponse>(content);
                    Debug.Log(result.status);
                    //result.EnsureSuccessStatusCode();
                    return result;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(queryVmRequestDto);

            var webRequest = new UnityWebRequest();
            webRequest.url = selectedAPI.GetEndpoint(EndpointNames.QueryVm);
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(raw));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<QueryVmResultDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    if (result.Data.Data.ReturnData == null)
                    {
                        throw new NullDataException(result.Data.Data.ReturnCode, result.Data.Data.ReturnMessage);
                    }
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var webRequest = new UnityWebRequest();
            webRequest.url = selectedAPI.GetEndpoint(EndpointNames.SendTransaction);
            Debug.Log("URL " + webRequest.url);
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(raw));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<CreateTransactionResponseDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }
        #endregion

        #region IMultiversXApiProvider 
        public async Task<T> GetRequest<T>(string url)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    return JsonSerializerWrapper.Deserialize<T>(content);
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

       

        public async Task<T> PostRequest<T>(string url, string jsonData)
        {
            var webRequest = new UnityWebRequest();
            webRequest.url = url;
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
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<T>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<T> GetWalletNfts<T>(string address)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetNFTsCount).Replace("{address}", address);
            int totalNfts = await GetRequest<int>(url);
            url = selectedAPI.GetEndpoint(EndpointNames.GetWalletNfts).Replace("{address}", address).Replace("{start}", "0").Replace("{totalNfts}", totalNfts.ToString());
            return await GetRequest<T>(url);
        }

        public async Task<T> GetWalletTokens<T>(string address)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetTokensCount).Replace("{address}", address);
            int totalTokens = await GetRequest<int>(url);
            url = selectedAPI.GetEndpoint(EndpointNames.GetWalletTokens).Replace("{address}", address).Replace("{start}", "0").Replace("{totalTokens}", totalTokens.ToString());  
            return await GetRequest<T>(url);
        }

        Task<TransactionDto> IElrondProvider.GetTransactionDetail(string txHash)
        {
            throw new System.NotImplementedException();
        }

        
        #endregion
    }
}