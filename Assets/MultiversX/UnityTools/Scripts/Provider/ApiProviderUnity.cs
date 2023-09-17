#define DebugAPI
#if DebugAPI
using UnityEngine;
#endif

using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain.Constants;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Provider.Dtos.API.Accounts;
using Mx.NET.SDK.Provider.Dtos.API.Blocks;
using Mx.NET.SDK.Provider.Dtos.API.Collections;
using Mx.NET.SDK.Provider.Dtos.API.Common;
using Mx.NET.SDK.Provider.Dtos.API.Network;
using Mx.NET.SDK.Provider.Dtos.API.NFTs;
using Mx.NET.SDK.Provider.Dtos.API.Tokens;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Provider.Dtos.API.xExchange;
using Mx.NET.SDK.Provider.Dtos.Common.QueryVm;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;


namespace MultiversX.UnityTools
{
    public class ApiProviderUnity : IApiProviderUnity
    {
        private string baseApiAddress;

        public ApiNetworkConfiguration NetworkConfiguration { get; }

        public ApiProviderUnity(ApiNetworkConfiguration config)
        {
            baseApiAddress = config.APIUri.AbsoluteUri;
            NetworkConfiguration = config;
        }

        #region IGenericProvider
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

        public async Task<TR> Post<TR>(string requestUri, object requestContent)
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


        #region IAccountsProvider
        public async Task<AccountDto> GetAccount(string address)
        {
            return await Get<AccountDto>($"accounts/{address}?withGuardianInfo=true");
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


        #region IBlocksProvider

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


        #region ICollectionsProvider

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


        #region INetworkProvider

        public async Task<NetworkConfigDto> GetNetworkConfig()
        {
            return await Get<NetworkConfigDto>("constants");
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


        #region INftsProvider

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


        #region ITokensProvider

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


        #region ITransactionsProvider

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

        public async Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequest)
        {
            return await Post<TransactionResponseDto>("transactions", transactionRequest);
        }

        public async Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequest)
        {
            return await Post<MultipleTransactionsResponseDto>("transaction/send-multiple", transactionsRequest);
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


        #region IUsernamesProvider

        public async Task<AccountDto> GetAccountByUsername(string username)
        {
            return await Get<AccountDto>($"usernames/{username}");
        }

        #endregion


        #region IQueryProvider

        public async Task<QueryVmResponseDto> QueryVm(QueryVmRequestDto queryRequestDto)
        {
            return await Post<QueryVmResponseDto>("query", queryRequestDto);
        }

        #endregion


        #region IxExchangeProvider

        public async Task<MexEconomicsDto> GetMexEconomics()
        {
            return await Get<MexEconomicsDto>("mex/economics");
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