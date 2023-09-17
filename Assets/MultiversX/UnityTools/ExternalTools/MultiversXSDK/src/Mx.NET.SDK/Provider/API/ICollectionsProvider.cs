using Mx.NET.SDK.Provider.Dtos.API.Collection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface ICollectionsProvider
    {
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
        Task<Collection[]> GetCollections<Collection>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

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
        Task<Collection> GetCollection<Collection>(string collectionIdentifier);
    }
}
