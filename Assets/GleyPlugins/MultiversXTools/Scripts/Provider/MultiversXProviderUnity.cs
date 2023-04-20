using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.Provider.Dtos.API.Account;
using Mx.NET.SDK.Provider.Dtos.API.Collection;
using Mx.NET.SDK.Provider.Dtos.API.Common;
using Mx.NET.SDK.Provider.Dtos.API.Exchange;
using Mx.NET.SDK.Provider.Dtos.API.Network;
using Mx.NET.SDK.Provider.Dtos.API.NFT;
using Mx.NET.SDK.Provider.Dtos.API.Token;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Provider.Dtos.Gateway;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Dtos.Gateway.Query;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MultiversXUnityTools
{
    /// <summary>
    /// You can implement your own version of provider
    /// Used to download data from blockchain API
    /// </summary>
    public class MultiversXProviderUnity : IMultiversXApiProvider
    {
        API selectedAPI;
        public MultiversXProviderUnity(API api)
        {
            selectedAPI = api;
        }

        #region IElrondProvider
        public async Task<GatewayNetworkConfigDataDto> GetGatewayNetworkConfig()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(selectedAPI.GetEndpoint(EndpointNames.GetNetworkConfig));
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonWrapper.Deserialize<GatewayResponseDto<GatewayNetworkConfigDataDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new APIException($"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
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
                    var result = JsonWrapper.Deserialize<AccountDto>(content);
                    return result;
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
            }
        }

        public async Task<TransactionDto> GetTransaction(string txHash)
        {
            return await GetTransactionCustom<TransactionDto>(txHash);
        }

        public async Task<T> GetTransactionCustom<T>(string txHash)
        {
            string url = selectedAPI.GetEndpoint(EndpointNames.GetTransactionDetail).Replace("{txHash}", txHash);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest.Result response = await webRequest.SendWebRequest();
            var content = webRequest.downloadHandler.text;
            switch (response)
            {
                case UnityWebRequest.Result.Success:
                    var result = JsonWrapper.Deserialize<T>(content);
                    return result;
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
            }
        }

        public async Task<QueryVmResponseDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw = JsonWrapper.Serialize(queryVmRequestDto);

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
                    var result = JsonWrapper.Deserialize<GatewayResponseDto<QueryVmResponseDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new APIException($"{webRequest.error} url: {webRequest.uri.AbsoluteUri}");
            }
        }

        public async Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonWrapper.Serialize(transactionRequestDto);
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
                    var result = JsonWrapper.Deserialize<TransactionResponseDto>(content);
                    return result;
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
            }
        }

        public async Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionRequestDto)
        {
            var raw = JsonWrapper.Serialize(transactionRequestDto);
            var webRequest = new UnityWebRequest();
            webRequest.url = selectedAPI.GetEndpoint(EndpointNames.SendTransactions);
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
                    var result = JsonWrapper.Deserialize<GatewayResponseDto<MultipleTransactionsResponseDto>>(content);
                    result.EnsureSuccessStatusCode();
                    return result.Data;
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
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
                    return JsonWrapper.Deserialize<T>(content);
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
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
                    var result = JsonWrapper.Deserialize<T>(content);
                    return result;
                default:
                    throw new APIException(JsonWrapper.Deserialize<APIExceptionResponse>(content));
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

       

        public Task<TR> Get<TR>(string requestUri)
        {
            throw new System.NotImplementedException();
        }

        public Task<TR> Post<TR>(string requestUri, object requestContent)
        {
            throw new System.NotImplementedException();
        }

        public Task<TR> GetGW<TR>(string requestUri)
        {
            throw new System.NotImplementedException();
        }

        public Task<TR> PostGW<TR>(string requestUri, object requestContent)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountTokenDto[]> GetAccountTokens(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountToken[]> GetAccountTokensCustom<AccountToken>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetAccountTokensCount(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountTokenDto> GetAccountToken(string address, string tokenIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountToken> GetAccountTokenCustom<AccountToken>(string address, string tokenIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountCollectionRoleDto[]> GetAccountCollectionsRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetAccountCollectionsRoleCount(string address, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountCollectionRoleDto> GetAccountCollectionRole(string address, string collectionIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountTokenRoleDto[]> GetAccountTokensRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetAccountTokensRoleCount(string address, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountTokenRoleDto> GetAccountTokenRole(string address, string tokenIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountNftDto[]> GetAccountNFTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<NFT[]> GetAccountNFTsCustom<NFT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetAccountNFTsCount(string address, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountNftDto> GetAccountNFT(string address, string nftIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<NFT> GetAccountNFTCustom<NFT>(string address, string nftIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountMetaESDTDto[]> GetAccountMetaESDTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<MetaESDT[]> GetAccountMetaESDTsCustom<MetaESDT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetAccountMetaESDTsCount(string address, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountMetaESDTDto> GetAccountMetaESDT(string address, string metaEsdtIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<MetaESDT> GetAccountMetaESDTCustom<MetaESDT>(string address, string metaEsdtIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountSCStakeDto[]> GetAccountStake(string scAddress)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountContractDto[]> GetAccountContracts(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetAccountContractsCount(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountHistoryDto[]> GetAccountHistory(string address, int size = 100, int from = 0)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountHistoryTokenDto[]> GetAccountHistoryToken(string address, string tokenIdentifier, int size = 100, int from = 0)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionDto[]> GetCollections(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<Collection[]> GetCollectionsCustom<Collection>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetCollectionsCount(Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionDto> GetCollection(string collectionIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<Collection> GetCollectionCustom<Collection>(string collectionIdentifier)
        {
            throw new System.NotImplementedException();
        }

       

        public Task<GatewayNetworkEconomicsDataDto> GetGatewayNetworkEconomics()
        {
            throw new System.NotImplementedException();
        }

        public Task<NetworkEconomicsDto> GetNetworkEconomics()
        {
            throw new System.NotImplementedException();
        }

        public Task<NetworkStatsDto> GetNetworkStats()
        {
            throw new System.NotImplementedException();
        }

        public Task<NFTDto[]> GetNFTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<NFT[]> GetNFTsCustom<NFT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNFTsCount(Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<MetaESDTDto[]> GetMetaESDTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<MetaESDT[]> GetMetaESDTsCustom<MetaESDT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetMetaESDTsCount(Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<NFTDto> GetNFT(string nftIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<NFT> GetNFTCustom<NFT>(string nftIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<MetaESDTDto> GetMetaESDT(string metaEsdtIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<MetaESDT> GetMetaESDTCustom<MetaESDT>(string metaEsdtIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AddressBalanceDto[]> GetNFTAccounts(string nftIdentifier, int size = 100, int from = 0)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNFTAccountsCounter(string nftIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AddressBalanceDto[]> GetMetaESDTAccounts(string metaEsdtIdentifier, int size = 100, int from = 0)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetMetaESDTAccountsCounter(string metaEsdtIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<TokenDto[]> GetTokens(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<Token[]> GetTokensCustom<Token>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetTokensCount(Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<TokenDto> GetToken(string tokenIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<Token> GetTokenCustom<Token>(string tokenIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<AddressBalanceDto[]> GetTokenAccounts(string tokenIdentifier, int size = 100, int from = 0)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetTokenAccountsCounter(string tokenIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<TransactionDto[]> GetTransactions(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<Transaction[]> GetTransactionsCustom<Transaction>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        

        public Task<string> GetTransactionsCount(Dictionary<string, string> parameters = null)
        {
            throw new System.NotImplementedException();
        }

       

        public Task<AccountDto> GetAccountByUsername(string username)
        {
            throw new System.NotImplementedException();
        }

        public Task<MexEconomicsDto> GetMexEconomics()
        {
            throw new System.NotImplementedException();
        }

      
        #endregion
    }
}