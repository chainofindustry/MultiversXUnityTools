using Mx.NET.SDK.Provider.Dtos.API.xExchange;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface IxExchangeProvider
    {
        /// <summary>
        /// This endpoint allows one to query economics details of Maiar Exchange
        /// </summary>
        /// <returns><see cref="MexEconomicsDto"/></returns>
        Task<MexEconomicsDto> GetMexEconomics();
    }
}
