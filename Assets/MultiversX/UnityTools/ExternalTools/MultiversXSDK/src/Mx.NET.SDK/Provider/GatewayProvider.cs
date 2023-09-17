using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Provider.Dtos.Common.QueryVm;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using Mx.NET.SDK.Provider.Dtos.Gateway;
using Mx.NET.SDK.Provider.Dtos.Gateway.Addresses;
using Mx.NET.SDK.Provider.Dtos.Gateway.Blocks;
using Mx.NET.SDK.Provider.Dtos.Gateway.ESDTs;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Dtos.Gateway.Transactions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider
{
    public class GatewayProvider : IGatewayProvider
    {
        private readonly HttpClient _httpGatewayClient;
        public GatewayNetworkConfiguration NetworkConfiguration { get; }

        public GatewayProvider(GatewayNetworkConfiguration configuration, Dictionary<string, string> extraRequestHeaders = null)
        {
            NetworkConfiguration = configuration;

            _httpGatewayClient = new HttpClient
            {
                BaseAddress = configuration.GatewayUri
            };
            if (extraRequestHeaders != null)
            {
                foreach (var header in extraRequestHeaders)
                    _httpGatewayClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        #region generic
        public async virtual Task<TR> Get<TR>(string requestUri)
        {
            var uri = requestUri.StartsWith("/") ? requestUri.Substring(1) : requestUri;
            var response = await _httpGatewayClient.GetAsync($"{uri}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new APIException(content);

            var result = JsonWrapper.Deserialize<GatewayResponseDto<TR>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async virtual Task<TR> Post<TR>(string requestUri, object requestContent)
        {
            var uri = requestUri.StartsWith("/") ? requestUri.Substring(1) : requestUri;
            var raw = JsonWrapper.Serialize(requestContent);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpGatewayClient.PostAsync(uri, payload);

            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new APIException(content);

            var result = JsonWrapper.Deserialize<GatewayResponseDto<TR>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }
        #endregion

        #region addresses

        public async Task<AddressDataDto> GetAddress(string address)
        {
            return await Get<AddressDataDto>($"address/{address}");
        }

        public async Task<AddressGuardianDataDto> GetAddressGuardianData(string address)
        {
            return await Get<AddressGuardianDataDto>($"address/{address}/guardian-data");
        }

        public async Task<StorageValueDto> GetStorageValue(string address, string key, bool isHex = false)
        {
            if (!isHex) key = Converter.ToHexString(key);

            return await Get<StorageValueDto>($"address/{address}/key/{key}");
        }

        public async Task<AllStorageDto> GetAllStorageValues(string address)
        {
            return await Get<AllStorageDto>($"address/{address}/keys");
        }

        #endregion

        #region esdt

        public async Task<EsdtTokenDataDto> GetEsdtTokens(string address)
        {
            return await Get<EsdtTokenDataDto>($"address/{address}/esdt");
        }

        public async Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier)
        {
            return await Get<EsdtTokenData>($"address/{address}/esdt/{tokenIdentifier}");
        }

        #endregion

        #region transactions

        public async Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequest)
        {
            return await Post<TransactionResponseDto>("transaction/send", transactionRequest);
        }

        public async Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequest)
        {
            return await Post<MultipleTransactionsResponseDto>("transaction/send-multiple", transactionsRequest);
        }

        public async Task<TransactionCostResponseDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            return await Post<TransactionCostResponseDto>("transaction/cost", transactionRequestDto);
        }

        public async Task<TransactionDto> GetTransaction(string txHash, bool withResults = false)
        {
            return await GetTransaction<TransactionDto>(txHash, withResults);
        }

        public async Task<Transaction> GetTransaction<Transaction>(string txHash, bool withResults = false)
        {
            return await Get<Transaction>($"transaction/{txHash}?withResults={withResults}");
        }

        #endregion

        #region network

        public async Task<NetworkConfigDataDto> GetNetworkConfig()
        {
            return await Get<NetworkConfigDataDto>("network/config");
        }

        public async Task<NetworkEconomicsDataDto> GetNetworkEconomics()
        {
            return await Get<NetworkEconomicsDataDto>("network/economics");
        }

        public async Task<ShardStatusDto> GetShardStatus(long shard)
        {
            return await Get<ShardStatusDto>($"network/status/{shard}");
        }

        #endregion

        #region nodes


        #endregion

        #region blocks

        public async Task<BlockDto> GetBlockByNonce(long nonce, long shard, bool withTxs = false)
        {
            return await Get<BlockDto>($"/block/by-nonce/{nonce}?withTxs={withTxs}&withResults=true");
        }

        public async Task<BlockDto> GetBlockByHash(string hash, long shard, bool withTxs = false)
        {
            return await Get<BlockDto>($"/block/{shard}/by-hash/{hash}?withTxs={withTxs}");
        }

        public async Task<InternalBlockDto> GetInternalBlockByNonce(long nonce)
        {
            return await Get<InternalBlockDto>($"/internal/json/shardblock/by-nonce/{nonce}");
        }

        public async Task<InternalBlockDto> GetInternalBlockByHash(string hash)
        {
            return await Get<InternalBlockDto>($"/internal/json/shardblock/by-hash/{hash}");
        }

        #endregion

        #region queryVM

        public async Task<QueryVmResponseDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            return await Post<QueryVmResponseDataDto>("vm-values/query", queryVmRequestDto);
        }

        #endregion
    }
}