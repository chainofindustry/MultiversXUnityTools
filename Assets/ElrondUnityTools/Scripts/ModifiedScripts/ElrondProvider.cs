using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Configuration;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;

namespace ElrondUnityTools
{
    public class ElrondProvider : IElrondProvider
    {
        private readonly HttpClient _httpClient;

        public ElrondProvider(HttpClient httpClient, ElrondNetworkConfiguration configuration = null)
        {
            _httpClient = httpClient;
            if (configuration != null)
            {
                _httpClient.BaseAddress = configuration.GatewayUri;
            }
        }

        public async Task<ConfigDataDto> GetNetworkConfig()
        {
            var response = await _httpClient.GetAsync("network1/config");

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<ConfigDataDto>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<AccountDto> GetAccount(string address)
        {
            var response = await _httpClient.GetAsync($"address1/{address}");

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<AccountDataDto>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data.Account;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<EsdtTokenDataDto> GetEsdtTokens(string address)
        {
            var response = await _httpClient.GetAsync($"address1/{address}/esdt");

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtTokenDataDto>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<EsdtItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId)
        {
            var response = await _httpClient.GetAsync($"address1/{address}/nft/{tokenIdentifier}/nonce/{tokenId}");

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtItemDto>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier)
        {
            var response = await _httpClient.GetAsync($"address1/{address}/esdt/{tokenIdentifier}");

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtTokenData>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction1/send", payload);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<CreateTransactionResponseDataDto>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction1/cost", payload);

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<TransactionCostDataDto>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(queryVmRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("vm-values1/query", payload);

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<QueryVmResultDataDto>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }

        public async Task<TransactionDto> GetTransactionDetail(string txHash)
        {
            var response = await _httpClient.GetAsync($"transaction1/{txHash}?withResults=true");

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<TransactionResponseData>>(content);
                result.EnsureSuccessStatusCode();
                return result.Data.Transaction;
            }
            else
            {
                throw new GatewayException(content, $"{response.StatusCode} url: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
        }
    }
}
