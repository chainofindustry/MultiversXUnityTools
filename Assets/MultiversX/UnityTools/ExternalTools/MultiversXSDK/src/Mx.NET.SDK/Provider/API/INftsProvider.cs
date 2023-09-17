using Mx.NET.SDK.Provider.Dtos.API.Common;
using Mx.NET.SDK.Provider.Dtos.API.NFT;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface INftsProvider
    {
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
        Task<NFT[]> GetNFTs<NFT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

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
        Task<MetaESDT[]> GetMetaESDTs<MetaESDT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

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
        Task<NFT> GetNFT<NFT>(string nftIdentifier);

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
        Task<MetaESDT> GetMetaESDT<MetaESDT>(string metaEsdtIdentifier);

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
        Task<string> GetNFTAccountsCount(string nftIdentifier);

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
        Task<string> GetMetaESDTAccountsCount(string metaEsdtIdentifier);
    }
}
