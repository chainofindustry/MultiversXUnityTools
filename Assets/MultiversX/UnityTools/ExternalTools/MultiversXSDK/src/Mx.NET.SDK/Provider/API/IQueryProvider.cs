using Mx.NET.SDK.Provider.Dtos.Common.QueryVm;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface IQueryProvider
    {
        /// <summary>
        /// Performs a vm query on a given smart contract and returns its results from API.
        /// </summary>
        /// <param name="queryRequestDto"></param>
        /// <returns><see cref="QueryVmResponseDto"/></returns>
        Task<QueryVmResponseDto> QueryVm(QueryVmRequestDto queryRequestDto);
    }
}
