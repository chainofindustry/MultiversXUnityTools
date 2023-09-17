namespace Mx.NET.SDK.Provider.Dtos.API.Network
{
    public class NetworkEconomicsDto
    {
        public long TotalSupply { get; set; }
        public long CirculatingSupply { get; set; }
        public long Staked { get; set; }
        public double Price { get; set; }
        public long MarketCap { get; set; }
        public double Apr { get; set; }
        public double TotalUpApr { get; set; }
        public double BaseApr { get; set; }
        public long TokenMarketCap { get; set; }
    }
}
