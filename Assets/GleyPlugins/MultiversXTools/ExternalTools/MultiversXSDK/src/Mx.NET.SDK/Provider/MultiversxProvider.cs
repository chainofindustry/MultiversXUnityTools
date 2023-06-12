using Mx.NET.SDK.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Mx.NET.SDK.Provider.Dtos.Gateway;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Dtos.API.Account;
using Mx.NET.SDK.Provider.Dtos.API.Collection;
using Mx.NET.SDK.Provider.Dtos.API.Network;
using Mx.NET.SDK.Provider.Dtos.API.NFT;
using Mx.NET.SDK.Provider.Dtos.API.Exchange;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Provider.Dtos.API.Common;
using Mx.NET.SDK.Provider.Dtos.API.Token;
using Mx.NET.SDK.Core.Domain.Constants;
using Mx.NET.SDK.Provider.Dtos.Gateway.Query;
using System.Net;
using Mx.NET.SDK.Provider.Dtos.API.Block;

namespace Mx.NET.SDK.Provider
{
    public class MultiversxProvider : IMultiversxProvider
    {
        private readonly HttpClient _httpAPIClient;
        private readonly HttpClient _httpGatewayClient;
        public readonly MultiversxNetworkConfiguration NetworkConfiguration;

        public MultiversxProvider(MultiversxNetworkConfiguration configuration)
        {
            NetworkConfiguration = configuration;

            _httpAPIClient = new HttpClient
            {
                BaseAddress = configuration.APIUri
            };

            _httpGatewayClient = new HttpClient
            {
                BaseAddress = configuration.GatewayUri
            };
        }

        #region generic

        public async Task<TR> Get<TR>(string requestUri)
        {
            var uri = requestUri.StartsWith("/") ? requestUri.Substring(1) : requestUri;
            var response = await _httpAPIClient.GetAsync($"{uri}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));

            var result = JsonWrapper.Deserialize<TR>(content);
            return result;
        }

        public async Task<TR> Post<TR>(string requestUri, object requestContent)
        {
            var uri = requestUri.StartsWith("/") ? requestUri.Substring(1) : requestUri;
            var raw = JsonWrapper.Serialize(requestContent);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpAPIClient.PostAsync(uri, payload);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));

            var result = JsonWrapper.Deserialize<TR>(content);
            return result;
        }

        public async Task<TR> GetGW<TR>(string requestUri)
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

        public async Task<TR> PostGW<TR>(string requestUri, object requestContent)
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

        #endregion region

        #region accounts

        public async Task<AccountDto> GetAccount(string address)
        {
            return await Get<AccountDto>($"accounts/{address}");
        }

        public async Task<AccountTokenDto[]> GetAccountTokens(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetAccountTokens<AccountTokenDto>(address, size, from, parameters);
        }

        public async Task<AccountToken[]> GetAccountTokens<AccountToken>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<AccountToken[]>($"accounts/{address}/tokens?from={from}&size={size}{args}");
        }

        public async Task<string> GetAccountTokensCount(string address)
        {
            return await Get<string>($"accounts/{address}/tokens/count");
        }

        public async Task<AccountTokenDto> GetAccountToken(string address, string tokenIdentifier)
        {
            return await GetAccountToken<AccountTokenDto>(address, tokenIdentifier);
        }

        public async Task<Token> GetAccountToken<Token>(string address, string tokenIdentifier)
        {
            return await Get<Token>($"accounts/{address}/tokens/{tokenIdentifier}");
        }

        public async Task<AccountCollectionRoleDto[]> GetAccountCollectionsRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<AccountCollectionRoleDto[]>($"accounts/{address}/roles/collections?from={from}&size={size}{args}");
        }

        public async Task<string> GetAccountCollectionsRoleCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"accounts/{address}/roles/collections/count{args}");
        }

        public async Task<AccountCollectionRoleDto> GetAccountCollectionRole(string address, string collectionIdentifier)
        {
            return await Get<AccountCollectionRoleDto>($"accounts/{address}/roles/collections/{collectionIdentifier}");
        }

        public async Task<AccountTokenRoleDto[]> GetAccountTokensRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<AccountTokenRoleDto[]>($"accounts/{address}/roles/tokens?from={from}&size={size}{args}");
        }

        public async Task<string> GetAccountTokensRoleCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"accounts/{address}/roles/tokens/count{args}");
        }

        public async Task<AccountTokenRoleDto> GetAccountTokenRole(string address, string tokenIdentifier)
        {
            return await Get<AccountTokenRoleDto>($"accounts/{address}/roles/tokens/{tokenIdentifier}");
        }

        public async Task<AccountNftDto[]> GetAccountNFTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetAccountNFTs<AccountNftDto>(address, size, from, parameters);
        }

        public async Task<NFT[]> GetAccountNFTs<NFT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<NFT[]>($"accounts/{address}/nfts?excludeMetaESDT=true&from={from}&size={size}{args}");
        }

        public async Task<string> GetAccountNFTsCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"accounts/{address}/nfts/count?excludeMetaESDT=true{args}");
        }

        public async Task<AccountNftDto> GetAccountNFT(string address, string nftIdentifier)
        {
            return await GetAccountNFT<AccountNftDto>(address, nftIdentifier);
        }

        public async Task<NFT> GetAccountNFT<NFT>(string address, string nftIdentifier)
        {
            return await Get<NFT>($"accounts/{address}/nfts/{nftIdentifier}");
        }

        public async Task<AccountMetaESDTDto[]> GetAccountMetaESDTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetAccountMetaESDTs<AccountMetaESDTDto>(address, size, from, parameters);
        }

        public async Task<MetaESDT[]> GetAccountMetaESDTs<MetaESDT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<MetaESDT[]>($"accounts/{address}/nfts?type={ESDTTokenType.MetaESDT}&from={from}&size={size}{args}");
        }

        public async Task<string> GetAccountMetaESDTsCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"accounts/{address}/nfts/count?type={ESDTTokenType.MetaESDT}{args}");
        }

        public async Task<AccountMetaESDTDto> GetAccountMetaESDT(string address, string metaEsdtIdentifier)
        {
            return await GetAccountMetaESDT<AccountMetaESDTDto>(address, metaEsdtIdentifier);
        }

        public async Task<MetaESDT> GetAccountMetaESDT<MetaESDT>(string address, string metaEsdtIdentifier)
        {
            return await Get<MetaESDT>($"accounts/{address}/nfts/{metaEsdtIdentifier}");
        }

        public async Task<AccountSCStakeDto[]> GetAccountStake(string address)
        {
            return await Get<AccountSCStakeDto[]>($"accounts/{address}/stake");
        }

        public async Task<AccountContractDto[]> GetAccountContracts(string address)
        {
            return await Get<AccountContractDto[]>($"accounts/{address}/contracts");
        }

        public async Task<string> GetAccountContractsCount(string address)
        {
            return await Get<string>($"accounts/{address}/contracts/count");
        }

        public async Task<AccountHistoryDto[]> GetAccountHistory(string address, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            return await Get<AccountHistoryDto[]>($"accounts/{address}/history?from={from}&size={size}");
        }

        public async Task<AccountHistoryTokenDto[]> GetAccountHistoryToken(string address, string tokenIdentifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            return await Get<AccountHistoryTokenDto[]>($"accounts/{address}/history/{tokenIdentifier}?from={from}&size={size}");
        }

        #endregion

        #region blocks

        public async Task<BlocksDto[]> GetBlocks(int size = 25, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetBlocks<BlocksDto>(size, from, parameters);
        }

        public async Task<Blocks[]> GetBlocks<Blocks>(int size = 25, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<Blocks[]>($"blocks?from={from}&size={size}{args}");
        }

        public async Task<string> GetBlocksCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"blocks/count{args}");
        }

        public async Task<BlockDto> GetBlock(string blockHash)
        {
            return await GetBlock<BlockDto>(blockHash);
        }

        public async Task<Block> GetBlock<Block>(string blockHash)
        {
            return await Get<Block>($"blocks/{blockHash}");
        }

        #endregion

        #region collections

        public async Task<CollectionDto[]> GetCollections(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetCollections<CollectionDto>(size, from, parameters);
        }

        public async Task<Collection[]> GetCollections<Collection>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<Collection[]>($"collections?from={from}&size={size}{args}");
        }

        public async Task<string> GetCollectionsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"collections/count{args}");
        }

        public async Task<CollectionDto> GetCollection(string collectionIdentifier)
        {
            return await GetCollection<CollectionDto>(collectionIdentifier);
        }

        public async Task<Collection> GetCollection<Collection>(string collectionIdentifier)
        {
            return await Get<Collection>($"collections/{collectionIdentifier}");
        }

        #endregion

        #region network

        public async Task<GatewayNetworkConfigDataDto> GetGatewayNetworkConfig()
        {
            return await GetGW<GatewayNetworkConfigDataDto>("network/config");
        }
        public async Task<GatewayNetworkEconomicsDataDto> GetGatewayNetworkEconomics()
        {
            return await GetGW<GatewayNetworkEconomicsDataDto>("network/economics");
        }

        public async Task<NetworkEconomicsDto> GetNetworkEconomics()
        {
            return await Get<NetworkEconomicsDto>("economics");
        }

        public async Task<NetworkStatsDto> GetNetworkStats()
        {
            return await Get<NetworkStatsDto>("stats");
        }

        #endregion

        #region nfts

        public async Task<NFTDto[]> GetNFTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetNFTs<NFTDto>(size, from, parameters);
        }

        public async Task<NFT[]> GetNFTs<NFT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<NFT[]>($"nfts?type={ESDTTokenType.NonFungibleESDT},{ESDTTokenType.SemiFungibleESDT}&from={from}&size={size}{args}");
        }

        public async Task<string> GetNFTsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"nfts/count?type={ESDTTokenType.NonFungibleESDT},{ESDTTokenType.SemiFungibleESDT}{args}");
        }

        public async Task<MetaESDTDto[]> GetMetaESDTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetMetaESDTs<MetaESDTDto>(size, from, parameters);
        }

        public async Task<MetaESDT[]> GetMetaESDTs<MetaESDT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<MetaESDT[]>($"nfts?type={ESDTTokenType.MetaESDT}&from={from}&size={size}{args}");
        }

        public async Task<string> GetMetaESDTsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"nfts/count?type={ESDTTokenType.MetaESDT}{args}");
        }

        public async Task<NFTDto> GetNFT(string nftIdentifier)
        {
            return await GetNFT<NFTDto>(nftIdentifier);
        }

        public async Task<NFT> GetNFT<NFT>(string nftIdentifier)
        {
            return await Get<NFT>($"nfts/{nftIdentifier}");
        }

        public async Task<MetaESDTDto> GetMetaESDT(string metaEsdtIdentifier)
        {
            return await GetMetaESDT<MetaESDTDto>(metaEsdtIdentifier);
        }

        public async Task<MetaESDT> GetMetaESDT<MetaESDT>(string metaEsdtIdentifier)
        {
            return await Get<MetaESDT>($"nfts/{metaEsdtIdentifier}");
        }

        public async Task<AddressBalanceDto[]> GetNFTAccounts(string nftIdentifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            return await Get<AddressBalanceDto[]>($"nfts/{nftIdentifier}/accounts?from={from}&size={size}");
        }

        public async Task<string> GetNFTAccountsCount(string nftIdentifier)
        {
            return await Get<string>($"nfts/{nftIdentifier}/accounts/count");
        }

        public async Task<AddressBalanceDto[]> GetMetaESDTAccounts(string metaEsdtIdentifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            return await Get<AddressBalanceDto[]>($"nfts/{metaEsdtIdentifier}/accounts?from={from}&size={size}");
        }

        public async Task<string> GetMetaESDTAccountsCount(string metaEsdtIdentifier)
        {
            return await Get<string>($"nfts/{metaEsdtIdentifier}/accounts/count");
        }

        #endregion

        #region tokens

        public async Task<TokenDto[]> GetTokens(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetTokens<TokenDto>(size, from, parameters);
        }

        public async Task<Token[]> GetTokens<Token>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<Token[]>($"tokens?from={from}&size={size}{args}");
        }

        public async Task<string> GetTokensCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"tokens/count{args}");
        }

        public async Task<TokenDto> GetToken(string tokenIdentifier)
        {
            return await GetToken<TokenDto>(tokenIdentifier);
        }

        public async Task<Token> GetToken<Token>(string tokenIdentifier)
        {
            return await Get<Token>($"tokens/{tokenIdentifier}");
        }

        public async Task<AddressBalanceDto[]> GetTokenAccounts(string tokenIdentifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            return await Get<AddressBalanceDto[]>($"tokens/{tokenIdentifier}/accounts?from={from}&size={size}");
        }

        public async Task<string> GetTokenAccountsCount(string tokenIdentifier)
        {
            return await Get<string>($"tokens/{tokenIdentifier}/accounts/count");
        }

        #endregion

        #region transactions

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

        public async Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            return await PostGW<TransactionResponseDto>("transaction/send", transactionRequestDto);
        }

        public async Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequestDto)
        {
            return await PostGW<MultipleTransactionsResponseDto>("transaction/send-multiple", transactionsRequestDto);
        }

        public async Task<string> GetTransactionsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";

            return await Get<string>($"transactions/count{args}");
        }

        public async Task<TransactionDto> GetTransaction(string txHash)
        {
            return await GetTransaction<TransactionDto>(txHash);
        }

        public async Task<Transaction> GetTransaction<Transaction>(string txHash)
        {
            return await Get<Transaction>($"transactions/{txHash}");
        }

        #endregion

        #region usernames

        public async Task<AccountDto> GetAccountByUsername(string username)
        {
            return await Get<AccountDto>($"usernames/{username}");
        }

        #endregion

        #region query

        public async Task<QueryVmResponseDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            return await PostGW<QueryVmResponseDto>("vm-values/query", queryVmRequestDto);
        }

        #endregion

        #region xExchange

        public async Task<MexEconomicsDto> GetMexEconomics()
        {
            return await Get<MexEconomicsDto>("mex/economics");
        }

        #endregion
    }
}
