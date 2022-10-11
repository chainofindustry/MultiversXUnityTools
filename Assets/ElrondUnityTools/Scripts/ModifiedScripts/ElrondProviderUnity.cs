using System.Threading.Tasks;
using Erdcsharp.Configuration;
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
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var content = webRequest.downloadHandler.text;
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<ConfigDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    Debug.LogError(webRequest.error);
                    return null;
            }
        }

        public async Task<AccountDto> GetAccount(string address)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(baseAddress.AbsoluteUri + $"address/{address}");
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var content = webRequest.downloadHandler.text;
                    var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<AccountDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data.Account;
                default:
                    Debug.LogError(webRequest.error);
                    return null;
            }
        }

        public Task<EsdtItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId)
        {
            throw new System.NotImplementedException();
        }

        public Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<EsdtTokenDataDto> GetEsdtTokens(string address)
        {
            throw new System.NotImplementedException();
        }



        public Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<TransactionDto> GetTransactionDetail(string txHash)
        {
            throw new System.NotImplementedException();
        }

        public Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            throw new System.NotImplementedException();
        }
    }
}