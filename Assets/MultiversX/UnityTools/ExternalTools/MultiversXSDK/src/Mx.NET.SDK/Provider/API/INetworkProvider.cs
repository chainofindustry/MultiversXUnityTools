using Mx.NET.SDK.Provider.Dtos.API.Network;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface INetworkProvider
    {
        /// <summary>
        /// Returns network-specific constants
        /// </summary>
        /// <returns><see cref="NetworkConfigDto"/></returns>
        Task<NetworkConfigDto> GetNetworkConfig();
        /// <summary>
        /// Returns general economics information
        /// </summary>
        /// <returns><see cref="NetworkEconomicsDto"/></returns>
        Task<NetworkEconomicsDto> GetNetworkEconomics();

        /// <summary>
        /// Returns general network statistics
        /// </summary>
        /// <returns><see cref="NetworkStatsDto"/></returns>
        Task<NetworkStatsDto> GetNetworkStats();
    }
}
