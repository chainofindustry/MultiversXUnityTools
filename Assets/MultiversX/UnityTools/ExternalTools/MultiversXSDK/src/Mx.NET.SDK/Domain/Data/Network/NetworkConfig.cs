using System.Threading.Tasks;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.Provider.Dtos.API.Network;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;

namespace Mx.NET.SDK.Domain.Data.Network
{
    public class NetworkConfig
    {
        public string ChainId { get; set; }
        public long GasPerDataByte { get; set; }
        public long MinGasLimit { get; set; }
        public long MinGasPrice { get; set; }
        public string GasPriceModifier { get; set; }
        public int MinTransactionVersion { get; set; }

        private NetworkConfig(NetworkConfigDataDto constants)
        {
            ChainId = constants.Config.erd_chain_id;
            GasPerDataByte = constants.Config.erd_gas_per_data_byte;
            MinGasLimit = constants.Config.erd_min_gas_limit;
            MinGasPrice = constants.Config.erd_min_gas_price;
            GasPriceModifier = constants.Config.erd_gas_price_modifier;
            MinTransactionVersion = constants.Config.erd_min_transaction_version;
        }

        private NetworkConfig(Provider.Dtos.API.Network.NetworkConfigDto constants)
        {
            ChainId = constants.ChainId;
            GasPerDataByte = constants.GasPerDataByte;
            MinGasLimit = constants.MinGasLimit;
            MinGasPrice = constants.MinGasPrice;
            GasPriceModifier = constants.GasPriceModifier;
            MinTransactionVersion = constants.MinTransactionVersion;
        }

        /// <summary>
        /// Get network-specific constants from Gateway
        /// </summary>
        /// <param name="provider">Gateway network provider</param>
        /// <returns>NetworkConfig</returns>
        public static async Task<NetworkConfig> GetFromNetwork(IGatewayProvider provider)
        {
            return new NetworkConfig(await provider.GetNetworkConfig());
        }

        /// <summary>
        /// Get network-specific constants from API
        /// </summary>
        /// <param name="provider">API network provider</param>
        /// <returns>NetworkConfig</returns>
        public static async Task<NetworkConfig> GetFromNetwork(IApiProvider provider)
        {
            return new NetworkConfig(await provider.GetNetworkConfig());
        }
    }
}
