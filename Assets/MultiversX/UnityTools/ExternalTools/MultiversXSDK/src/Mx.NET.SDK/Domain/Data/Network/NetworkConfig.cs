using System.Threading.Tasks;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Gateway;

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

        private NetworkConfig() { }

        private NetworkConfig(GatewayNetworkConfigDataDto constants)
        {
            ChainId = constants.Config.erd_chain_id;
            GasPerDataByte = constants.Config.erd_gas_per_data_byte;
            MinGasLimit = constants.Config.erd_min_gas_limit;
            MinGasPrice = constants.Config.erd_min_gas_price;
            GasPriceModifier = constants.Config.erd_gas_price_modifier;
            MinTransactionVersion = constants.Config.erd_min_transaction_version;
        }

        /// <summary>
        /// Synchronize the configuration with the network
        /// </summary>
        /// <param name="provider">Gateway network provider</param>
        /// <returns>NetworkConfig</returns>
        public static async Task<NetworkConfig> GetFromNetwork(IGatewayProvider provider)
        {
            return new NetworkConfig(await provider.GetGatewayNetworkConfig());
        }
    }
}
