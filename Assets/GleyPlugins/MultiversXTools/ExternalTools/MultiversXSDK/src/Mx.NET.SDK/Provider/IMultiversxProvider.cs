using Mx.NET.SDK.Provider.Dtos.API.Account;
using Mx.NET.SDK.Provider.Dtos.API.Collection;
using Mx.NET.SDK.Provider.Dtos.API.Common;
using Mx.NET.SDK.Provider.Dtos.API.Exchange;
using Mx.NET.SDK.Provider.Dtos.API.Network;
using Mx.NET.SDK.Provider.Dtos.API.NFT;
using Mx.NET.SDK.Provider.Dtos.API.Token;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Dtos.Gateway.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider
{
    public interface IMultiversxProvider
    {
        #region generic

        /// <summary>
        /// Generic GET request to API
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/economics?extract=price')</param>
        /// <returns></returns>
        Task<TR> Get<TR>(string requestUri);

        /// <summary>
        /// Generic POST request to API
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/transactions')</param>
        /// <param name="requestContent">Request content object (e.g. TransactionRequestDto object)</param>
        /// <returns></returns>
        Task<TR> Post<TR>(string requestUri, object requestContent);

        /// <summary>
        /// Generic GET request to Gateway
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/economics?extract=price')</param>
        /// <returns></returns>
        Task<TR> GetGW<TR>(string requestUri);

        /// <summary>
        /// Generic POST request to Gateway
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/transactions')</param>
        /// <param name="requestContent">Request content object (e.g. TransactionRequestDto object)</param>
        /// <returns></returns>
        Task<TR> PostGW<TR>(string requestUri, object requestContent);

        #endregion

        #region accounts

        /// <summary>
        /// Returns account details for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <returns><see cref="AccountDto"/></returns>
        Task<AccountDto> GetAccount(string address);

        /// <summary>
        /// Returns an array of all available fungible tokens for a given address, together with their balance
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="AccountTokenDto"/></returns>
        Task<AccountTokenDto[]> GetAccountTokens(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of all available fungible tokens for a given address, together with their balance
        /// </summary>
        /// <typeparam name="AccountToken">Custom DTO</typeparam>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<AccountToken[]> GetAccountTokensCustom<AccountToken>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the total number of tokens for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <returns></returns>
        Task<string> GetAccountTokensCount(string address);

        /// <summary>
        /// Returns details about a specific fungible token from a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <returns><see cref="AccountTokenDto"/></returns>
        Task<AccountTokenDto> GetAccountToken(string address, string tokenIdentifier);

        /// <summary>
        /// Returns details about a specific fungible token from a given address
        /// </summary>
        /// <typeparam name="AccountToken">Custom DTO</typeparam>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <returns></returns>
        Task<AccountToken> GetAccountTokenCustom<AccountToken>(string address, string tokenIdentifier);

        /// <summary>
        /// Returns NFT/SFT/MetaESDT collections where the account is owner or has some special roles assigned to it
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="AccountCollectionRoleDto"/></returns>
        Task<AccountCollectionRoleDto[]> GetAccountCollectionsRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the total number of NFT/SFT/MetaESDT collections where the account is owner or has some special roles assigned to it
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetAccountCollectionsRoleCount(string address, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns details about a specific NFT/SFT/MetaESDT collection if the account is owner or has some special roles assigned to it
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="collectionIdentifier">The collection identifier</param>
        /// <returns><see cref="AccountCollectionRoleDto"/></returns>
        Task<AccountCollectionRoleDto> GetAccountCollectionRole(string address, string collectionIdentifier);

        /// <summary>
        /// Returns fungible token roles where the account is owner or has some special roles assigned to it
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="AccountTokenRoleDto"/></returns>
        Task<AccountTokenRoleDto[]> GetAccountTokensRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the total number of fungible tokens where the account is owner or has some special roles assigned to it
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetAccountTokensRoleCount(string address, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns details about a specific fungible token if the account is owner or has some special roles assigned to it
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="tokenIdentifier">The collection identifier</param>
        /// <returns><see cref="AccountTokenRoleDto"/></returns>
        Task<AccountTokenRoleDto> GetAccountTokenRole(string address, string tokenIdentifier);

        /// <summary>
        /// Returns an array of owned NFTs/SFTs for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="NFTDto"/></returns>
        Task<AccountNftDto[]> GetAccountNFTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of owned NFTs/SFTs for a given address
        /// </summary>
        /// <typeparam name="NFT">Custom DTO</typeparam>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<NFT[]> GetAccountNFTsCustom<NFT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the total number of owned NFTs/SFTs for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetAccountNFTsCount(string address, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns details about a specific NFT/SFT owned by a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="nftIdentifier">Token identifier</param>
        /// <returns><see cref="NFTDto"/></returns>
        Task<AccountNftDto> GetAccountNFT(string address, string nftIdentifier);

        /// <summary>
        /// Returns details about a specific NFT/SFT owned by a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="nftIdentifier">The token identifier</param>
        /// <returns></returns>
        Task<NFT> GetAccountNFTCustom<NFT>(string address, string nftIdentifier);

        /// <summary>
        /// Returns an array of owned MetaESDTs for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="MetaESDTDto"/></returns>
        Task<AccountMetaESDTDto[]> GetAccountMetaESDTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of owned MetaESDTs for a given address
        /// </summary>
        /// <typeparam name="MetaESDT">Custom DTO</typeparam>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<MetaESDT[]> GetAccountMetaESDTsCustom<MetaESDT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the total number of owned MetaESDTs for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetAccountMetaESDTsCount(string address, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns details about a specific MetaESDT owned by a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="metaEsdtIdentifier">Token identifier</param>
        /// <returns><see cref="MetaESDTDto"/></returns>
        Task<AccountMetaESDTDto> GetAccountMetaESDT(string address, string metaEsdtIdentifier);

        /// <summary>
        /// Returns details about a specific MetaESDT owned by a given address
        /// </summary>
        /// <typeparam name="MetaESDT">Custom DTO</typeparam>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="metaEsdtIdentifier">The token identifier</param>
        /// <returns></returns>
        Task<MetaESDT> GetAccountMetaESDTCustom<MetaESDT>(string address, string metaEsdtIdentifier);

        /// <summary>
        /// Returns the total staked amount for the given provider, as well as when and how much unbond will be performed
        /// </summary>
        /// <param name="scAddress">Smart Contract address</param>
        /// <returns><see cref="AccountSCStakeDto"/></returns>
        Task<AccountSCStakeDto[]> GetAccountStake(string scAddress);

        /// <summary>
        /// Returns an array of deployed contracts for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <returns><see cref="AccountContractDto"/></returns>
        Task<AccountContractDto[]> GetAccountContracts(string address);

        /// <summary>
        /// Returns total number of deployed contracts for a given address
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <returns></returns>
        Task<string> GetAccountContractsCount(string address);

        /// <summary>
        /// Returns account EGLD balance history
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <returns><see cref="AccountHistoryDto"/></returns>
        Task<AccountHistoryDto[]> GetAccountHistory(string address, int size = 100, int from = 0);

        /// <summary>
        /// Returns account Token balance history
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="tokenIdentifier">The Token identifier/MetaESDT collection identifier</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <returns><see cref="AccountHistoryTokenDto"/></returns>
        Task<AccountHistoryTokenDto[]> GetAccountHistoryToken(string address, string tokenIdentifier, int size = 100, int from = 0);

        #endregion

        #region collections

        /// <summary>
        /// Returns an array of Collections
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="CollectionDto"/></returns>
        Task<CollectionDto[]> GetCollections(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of Collections
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<Collection[]> GetCollectionsCustom<Collection>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the counter of Collections
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetCollectionsCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the specified Collection
        /// </summary>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <returns><see cref="CollectionDto"/></returns>
        Task<CollectionDto> GetCollection(string collectionIdentifier);

        /// <summary>
        /// Returns the specified Collection
        /// </summary>
        /// <param name="collectionIdentifier">Collection identifier</param>
        /// <returns></returns>
        Task<Collection> GetCollectionCustom<Collection>(string collectionIdentifier);

        #endregion

        #region network

        /// <summary>
        /// This endpoint allows one to query basic details about the configuration of the Network.
        /// </summary>
        /// <returns><see cref="GatewayNetworkConfigDataDto"/></returns>
        Task<GatewayNetworkConfigDataDto> GetGatewayNetworkConfig();

        /// <summary>
        /// This endpoint allows one to query basic details about the economics of the Network.
        /// </summary>
        /// <returns><see cref="GatewayNetworkEconomicsDataDto"/></returns>
        Task<GatewayNetworkEconomicsDataDto> GetGatewayNetworkEconomics();

        /// <summary>
        /// This endpoint allows one to query economics information.
        /// </summary>
        /// <returns><see cref="NetworkEconomicsDto"/></returns>
        Task<NetworkEconomicsDto> GetNetworkEconomics();

        /// <summary>
        /// This endpoint allows one to query network statistics.
        /// </summary>
        /// <returns><see cref="NetworkStatsDto"/></returns>
        Task<NetworkStatsDto> GetNetworkStats();

        #endregion

        #region nfts

        /// <summary>
        /// Returns an array of Non-Fungible / Semi-Fungible tokens available on blockchain
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="NFTDto"/></returns>
        Task<NFTDto[]> GetNFTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of Non-Fungible / Semi-Fungible tokens available on blockchain
        /// </summary>
        /// <typeparam name="NFT">Custom DTO</typeparam>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<NFT[]> GetNFTsCustom<NFT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the total number of Non-Fungible / Semi-Fungible tokens
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetNFTsCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of MetaESDT tokens available on blockchain
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="MetaESDTDto"/></returns>
        Task<MetaESDTDto[]> GetMetaESDTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of MetaESDT tokens available on blockchain
        /// </summary>
        /// <typeparam name="MetaESDT">Custom DTO</typeparam>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        /// <summary>
        Task<MetaESDT[]> GetMetaESDTsCustom<MetaESDT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the total number of MetaESDT tokens
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetMetaESDTsCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the details of a Non-Fungible / Semi-Fungible token for a given identifier
        /// </summary>
        /// <param name="nftIdentifier">NFT identifier</param>
        /// <returns><see cref="NFTDto"/></returns>
        Task<NFTDto> GetNFT(string nftIdentifier);

        /// <summary>
        /// Returns the details of a Non-Fungible / Semi-Fungible token for a given identifier
        /// </summary>
        /// <typeparam name="NFT">Custom DTO</typeparam>
        /// <param name="identifier">NFT identifier</param>
        /// <returns></returns>
        Task<NFT> GetNFTCustom<NFT>(string nftIdentifier);

        /// <summary>
        /// Returns the details of a MetaESDT token for a given identifier
        /// </summary>
        /// <param name="metaEsdtIdentifier">MetaESDT identifier</param>
        /// <returns><see cref="MetaESDTDto"/></returns>
        Task<MetaESDTDto> GetMetaESDT(string metaEsdtIdentifier);

        /// <summary>
        /// Returns the details of a MetaESDT token for a given identifier
        /// </summary>
        /// <typeparam name="MetaESDT">Custom DTO</typeparam>
        /// <param name="metaEsdtIdentifier">MetaESDT identifier</param>
        /// <returns></returns>
        Task<MetaESDT> GetMetaESDTCustom<MetaESDT>(string metaEsdtIdentifier);

        /// <summary>
        /// Returns an array of addresses that hold balances for a specific Non-Fungible / Semi-Fungible token
        /// </summary>
        /// <param name="nftIdentifier">NFT Identifier</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <returns><see cref="AddressBalanceDto"/></returns>
        Task<AddressBalanceDto[]> GetNFTAccounts(string nftIdentifier, int size = 100, int from = 0);

        /// <summary>
        /// Returns the number of addresses that hold balances for a specific Non-Fungible / Semi-Fungible token
        /// </summary>
        /// <param name="nftIdentifier">NFT Identifier</param>
        /// <returns></returns>
        Task<string> GetNFTAccountsCounter(string nftIdentifier);

        /// <summary>
        /// Returns an array of addresses that hold balances for a specific MetaESDT token
        /// </summary>
        /// <param name="metaEsdtIdentifier">MetaESDT Identifier</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <returns><see cref="AddressBalanceDto"/></returns>
        Task<AddressBalanceDto[]> GetMetaESDTAccounts(string metaEsdtIdentifier, int size = 100, int from = 0);

        /// <summary>
        /// Returns the number of addresses that hold balances for a specific MetaESDT token
        /// </summary>
        /// <param name="metaEsdtIdentifier">Identifier</param>
        /// <returns></returns>
        Task<string> GetMetaESDTAccountsCounter(string metaEsdtIdentifier);

        #endregion

        #region tokens

        /// <summary>
        /// Returns an array of Tokens
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="TokenDto"/></returns>
        Task<TokenDto[]> GetTokens(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of Tokens
        /// </summary>
        /// <typeparam name="Token">Custom DTO</typeparam>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<Token[]> GetTokensCustom<Token>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the counter of Tokens
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetTokensCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the specified Token
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <returns><see cref="TokenDto"/></returns>
        Task<TokenDto> GetToken(string tokenIdentifier);

        /// <summary>
        /// Returns the specified Token as custom object.
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <returns></returns>
        Task<Token> GetTokenCustom<Token>(string tokenIdentifier);

        /// <summary>
        /// Returns the accounts and balance for specified Token
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <returns><see cref="AccountTokenDto"/></returns>
        Task<AddressBalanceDto[]> GetTokenAccounts(string tokenIdentifier, int size = 100, int from = 0);

        /// <summary>
        /// Returns the number of accounts holding the specified Token
        /// </summary>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <returns></returns>
        Task<string> GetTokenAccountsCounter(string tokenIdentifier);

        #endregion

        #region transactions

        /// <summary>
        /// Returns the transactions satisfying the parameters
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="TransactionDto"/></returns>
        Task<TransactionDto[]> GetTransactions(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the transactions satisfying the parameters
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns>Array of your custom Transaction objects</returns>
        Task<Transaction[]> GetTransactionsCustom<Transaction>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// This endpoint allows one to send a signed Transaction to the Blockchain.
        /// </summary>
        /// <param name="transactionRequest">The transaction payload</param>
        /// <returns>TxHash</returns>
        Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequest);

        /// <summary>
        /// This endpoint allows one to send multiple signed Transactions to the Blockchain.
        /// </summary>
        /// <param name="transactionsRequest">Array of transactions payload</param>
        /// <returns><see cref="MultipleTransactionsResponseDto"/></returns>
        Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequest);

        /// <summary>
        /// Returns the counter of transactions from blockchain.
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetTransactionsCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// This endpoint allows one to query the details of a Transaction.
        /// </summary>
        /// <param name="txHash">The transaction hash</param>
        /// <returns><see cref="TransactionDto"/></returns>
        Task<TransactionDto> GetTransaction(string txHash);

        /// <summary>
        /// This endpoint allows one to query the details of a Transaction.
        /// </summary>
        /// <param name="txHash">The transaction hash</param>
        /// <returns>Your custom Transaction object</returns>
        Task<Transaction> GetTransactionCustom<Transaction>(string txHash);

        #endregion

        #region usernames

        Task<AccountDto> GetAccountByUsername(string username);

        #endregion

        #region query

        /// <summary>
        /// This endpoint allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output).
        /// </summary>
        /// <param name="queryVmRequestDto"></param>
        /// <returns><see cref="QueryVmResponseDto"/></returns>
        Task<QueryVmResponseDto> QueryVm(QueryVmRequestDto queryVmRequestDto);

        #endregion

        #region xExchange

        /// <summary>
        /// This endpoint allows one to query economics details of Maiar Exchange
        /// </summary>
        /// <returns><see cref="MexEconomicsDto"/></returns>
        Task<MexEconomicsDto> GetMexEconomics();

        #endregion
    }
}
