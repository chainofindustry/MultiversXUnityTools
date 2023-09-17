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
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace MultiversX.UnityTools
{
    public class GatewayProviderUnity : IGatewayProviderUnity
    {
        string baseGatewayAddress;

        public GatewayNetworkConfiguration NetworkConfiguration { get; }

        public GatewayProviderUnity(GatewayNetworkConfiguration config)
        {
            baseGatewayAddress = config.GatewayUri.AbsoluteUri;
            NetworkConfiguration = config;
        }


        #region IGenericProvider
        public async Task<TR> Get<TR>(string requestUri)
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


        public async Task<TR> Post<TR>(string requestUri, object requestContent)
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


        #region INetworkProvider
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


        #region IAddressesProvider

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


        #region IESDTProvider

        public async Task<EsdtTokenDataDto> GetEsdtTokens(string address)
        {
            return await Get<EsdtTokenDataDto>($"address/{address}/esdt");
        }

        public async Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier)
        {
            return await Get<EsdtTokenData>($"address/{address}/esdt/{tokenIdentifier}");
        }

        #endregion


        #region ITransactionsProvider

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


        #region INodesProvider


        #endregion


        #region IBlocksProvider

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


        #region IQueryVmProvider

        public async Task<QueryVmResponseDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            return await Post<QueryVmResponseDataDto>("vm-values/query", queryVmRequestDto);
        }

        #endregion
    }
}