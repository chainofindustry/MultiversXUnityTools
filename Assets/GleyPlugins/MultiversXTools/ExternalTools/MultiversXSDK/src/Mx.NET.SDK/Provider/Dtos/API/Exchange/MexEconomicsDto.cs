namespace Mx.NET.SDK.Provider.Dtos.API.Exchange
{
    public class MexEconomicsDto
    {
        public long TotalSupply { get; set; }
        public long CirculatingSupply { get; set; }
        public double Price { get; set; }
        public long MarketCap { get; set; }
        public long Volume24h { get; set; }
        public long MarketPairs { get; set; }
    }
}
