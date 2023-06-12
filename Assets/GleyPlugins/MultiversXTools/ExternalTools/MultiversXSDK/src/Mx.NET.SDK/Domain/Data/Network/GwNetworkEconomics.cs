using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Gateway;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Domain.Data.Network
{
    public class GwNetworkEconomics
    {
        public string DevRewards { get; set; }
        public int EpochForEconomicsData { get; set; }
        public string Inflation { get; set; }
        public string TotalBaseStakedValue { get; set; }
        public string TotalFees { get; set; }
        public string TotalSupply { get; set; }
        public string TotalTopUpValue { get; set; }

        private GwNetworkEconomics(GatewayNetworkEconomicsDataDto economics)
        {
            DevRewards = economics.Metrics.erd_dev_rewards;
            EpochForEconomicsData = economics.Metrics.erd_epoch_for_economics_data;
            Inflation = economics.Metrics.erd_inflation;
            TotalBaseStakedValue = economics.Metrics.erd_total_base_staked_value;
            TotalFees = economics.Metrics.erd_total_fees;
            TotalSupply = economics.Metrics.erd_total_supply;
            TotalTopUpValue = economics.Metrics.erd_total_top_up_value;
        }

        /// <summary>
        /// Synchronize the economics with the network
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <returns>NetworkEconomics</returns>
        public static async Task<GwNetworkEconomics> GetFromNetwork(INetworkProvider provider)
        {
            return new GwNetworkEconomics(await provider.GetGatewayNetworkEconomics());
        }
    }
}
