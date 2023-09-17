using Mx.NET.SDK.Provider.API;
using Mx.NET.SDK.Provider.Dtos.API.Network;
using System.Numerics;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Domain.Data.Network
{
    public class NetworkEconomicsMvx
    {
        public long TotalSupply { get; set; }
        public long CirculatingSupply { get; set; }
        public long Staked { get; set; }
        public double Price { get; set; }
        public long MarketCap { get; set; }
        public double Apr { get; set; }
        public double TotalUpApr { get; set; }
        public double BaseApr { get; set; }
        public BigInteger TokenMarketCap { get; set; }

        private NetworkEconomicsMvx(NetworkEconomicsDto economics)
        {
            TotalSupply = economics.TotalSupply;
            CirculatingSupply = economics.CirculatingSupply;
            Staked = economics.Staked;
            Price = economics.Price;
            MarketCap = economics.MarketCap;
            Apr = economics.Apr;
            TotalUpApr = economics.TotalUpApr;
            BaseApr = economics.BaseApr;
            TokenMarketCap = economics.TokenMarketCap;
        }

        /// <summary>
        /// Get general economics information from API
        /// </summary>
        /// <param name="provider">Network provider</param>
        /// <returns>Economics</returns>
        public static async Task<NetworkEconomicsMvx> GetFromNetwork(INetworkProvider provider)
        {
            return new NetworkEconomicsMvx(await provider.GetNetworkEconomics());
        }
    }
}
