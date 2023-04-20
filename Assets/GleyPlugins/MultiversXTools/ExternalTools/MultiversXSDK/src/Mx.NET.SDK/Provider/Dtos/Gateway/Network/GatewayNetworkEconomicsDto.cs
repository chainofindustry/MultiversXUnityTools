namespace Mx.NET.SDK.Provider.Dtos.Gateway.Network
{
    public class GatewayNetworkEconomicsDataDto
    {
        public GatewayNetworkEconomicsDto Metrics { get; set; }
    }

    public class GatewayNetworkEconomicsDto
    {
        public string erd_dev_rewards { get; set; }
        public int erd_epoch_for_economics_data { get; set; }
        public string erd_inflation { get; set; }
        public string erd_total_base_staked_value { get; set; }
        public string erd_total_fees { get; set; }
        public string erd_total_supply { get; set; }
        public string erd_total_top_up_value { get; set; }
    }
}
