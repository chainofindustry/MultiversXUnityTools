using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Configuration;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using UnityEngine;
using UnityEngine.Networking;

namespace ElrondUnityTools
{
    public class ElrondProviderUnity : IElrondProvider
    {
        private System.Uri baseAddress;

        public ElrondProviderUnity(ElrondNetworkConfiguration configuration)
        {
            if (configuration != null)
            {
                baseAddress = configuration.GatewayUri;
                Debug.Log(baseAddress.AbsoluteUri);
            }
        }

        public async Task<ConfigDataDto> GetNetworkConfig()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(baseAddress.AbsoluteUri + "network/config");
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
            UnityWebRequest webRequest = UnityWebRequest.Get(baseAddress.AbsoluteUri + $"address/{address}");
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
            UnityWebRequest webRequest = UnityWebRequest.Get(baseAddress.AbsoluteUri + $"address1/{address}/nft/{tokenIdentifier}/nonce/{tokenId}");
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
            UnityWebRequest webRequest = UnityWebRequest.Get(baseAddress.AbsoluteUri + $"address1/{address}/esdt/{tokenIdentifier}");
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
            UnityWebRequest webRequest = UnityWebRequest.Get(baseAddress.AbsoluteUri + $"address1/{address}/esdt");
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
            webRequest.url = baseAddress.AbsoluteUri + "transaction1/cost";
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

        public async Task<TransactionDto> GetTransactionDetail(string txHash)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(baseAddress.AbsoluteUri + $"transaction/{txHash}?withResults=true");
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<TransactionResponseData>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data.Transaction;
                default:
                    throw new GatewayException(content, $"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(queryVmRequestDto);

            var webRequest = new UnityWebRequest();
            webRequest.url = baseAddress.AbsoluteUri + "vm-values/query";
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
                    if(result.Data.Data.ReturnData==null)
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
            webRequest.url = baseAddress.AbsoluteUri + "transaction/send";
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
    }
}