using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.Gateway
{
    public interface INetworkProvider
    {
        /// <summary>
        /// This endpoint allows one to query basic details about the configuration of the Network.
        /// </summary>
        /// <returns><see cref="NetworkConfigDataDto"/></returns>
        Task<NetworkConfigDataDto> GetNetworkConfig();

        /// <summary>
        /// This endpoint allows one to query basic details about the economics of the Network.
        /// </summary>
        /// <returns><see cref="NetworkEconomicsDataDto"/></returns>
        Task<NetworkEconomicsDataDto> GetNetworkEconomics();

        /// <summary>
        /// This endpoint allows one to query the status of a given Shard.
        /// </summary>
        /// <param name="shard">Shard</param>
        /// <returns><see cref="ShardStatusDto"/></returns>
        Task<ShardStatusDto> GetShardStatus(long shard);
    }
}
