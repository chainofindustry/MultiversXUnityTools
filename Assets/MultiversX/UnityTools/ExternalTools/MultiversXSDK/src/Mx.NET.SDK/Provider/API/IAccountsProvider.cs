using Mx.NET.SDK.Provider.Dtos.API.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface IAccountsProvider
    {
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
        Task<AccountToken[]> GetAccountTokens<AccountToken>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

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
        Task<AccountToken> GetAccountToken<AccountToken>(string address, string tokenIdentifier);

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
        Task<NFT[]> GetAccountNFTs<NFT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

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
        Task<NFT> GetAccountNFT<NFT>(string address, string nftIdentifier);

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
        Task<MetaESDT[]> GetAccountMetaESDTs<MetaESDT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null);

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
        Task<MetaESDT> GetAccountMetaESDT<MetaESDT>(string address, string metaEsdtIdentifier);

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
    }
}
