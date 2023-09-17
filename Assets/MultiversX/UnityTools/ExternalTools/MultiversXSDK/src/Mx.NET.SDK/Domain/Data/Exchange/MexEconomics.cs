using Mx.NET.SDK.Provider.API;
using Mx.NET.SDK.Provider.Dtos.API.Exchange;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Domain.Data.Exchange
{
    public class MexEconomics
    {
        public long TotalSupply { get; set; }
        public long CirculatingSupply { get; set; }
        public double Price { get; set; }
        public long MarketCap { get; set; }
        public long Volume24h { get; set; }
        public long MarketPairs { get; set; }

        private MexEconomics() { }

        private MexEconomics(MexEconomicsDto economics)
        {
            TotalSupply = economics.TotalSupply;
            CirculatingSupply = economics.CirculatingSupply;
            Price = economics.Price;
            MarketCap = economics.MarketCap;
            Volume24h = economics.Volume24h;
            MarketPairs = economics.MarketPairs;
        }

        /// <summary>
        /// Gets the Network Mex Economics
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <returns>MexEconomics</returns>
        public static async Task<MexEconomics> GetFromNetwork(IxExchangeProvider provider)
        {
            return new MexEconomics(await provider.GetMexEconomics());
        }

        /// <summary>
        /// New empty Economics
        /// </summary>
        /// <returns>MexEconomics</returns>
        public static MexEconomics New()
        {
            return new MexEconomics();
        }
    }
}
