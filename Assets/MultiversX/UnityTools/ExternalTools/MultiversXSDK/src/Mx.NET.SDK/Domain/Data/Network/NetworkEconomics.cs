using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Gateway;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Domain.Data.Network
{
    public class NetworkEconomics
    {
        public string DevRewards { get; set; }
        public int EpochForEconomicsData { get; set; }
        public string Inflation { get; set; }
        public string TotalBaseStakedValue { get; set; }
        public string TotalFees { get; set; }
        public string TotalSupply { get; set; }
        public string TotalTopUpValue { get; set; }

        private NetworkEconomics(NetworkEconomicsDataDto economics)
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
        /// Get general economics information from API
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <returns>NetworkEconomics</returns>
        public static async Task<NetworkEconomics> GetFromNetwork(INetworkProvider provider)
        {
            return new NetworkEconomics(await provider.GetNetworkEconomics());
        }
    }
}
