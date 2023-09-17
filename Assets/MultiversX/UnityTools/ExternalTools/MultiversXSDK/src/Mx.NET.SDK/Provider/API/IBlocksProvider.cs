using Mx.NET.SDK.Provider.Dtos.API.Blocks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface IBlocksProvider
    {
        /// <summary>
        /// Returns an array of Blocks
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="BlocksDto"/></returns>
        Task<BlocksDto[]> GetBlocks(int size = 25, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns an array of Blocks
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="BlocksDto"/></returns>
        Task<Blocks[]> GetBlocks<Blocks>(int size = 25, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the counter of Blocks
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetBlocksCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns a Block
        /// </summary>
        /// <param name="blockHash">Block Hash</param>
        /// <returns><see cref="BlocksDto"/></returns>
        Task<BlockDto> GetBlock(string blockHash);

        /// <summary>
        /// Returns a Block
        /// </summary>
        /// <param name="blockHash">Block Hash</param>
        /// <returns><see cref="BlocksDto"/></returns>
        Task<Block> GetBlock<Block>(string blockHash);
    }
}
