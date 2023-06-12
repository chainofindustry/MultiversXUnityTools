using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.Gateway
{
    public interface INetworkProvider
    {
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
    }
}
