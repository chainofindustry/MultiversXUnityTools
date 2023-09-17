using Mx.NET.SDK.Provider.Dtos.API.Network;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface INetworkProvider
    {
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
    }
}
