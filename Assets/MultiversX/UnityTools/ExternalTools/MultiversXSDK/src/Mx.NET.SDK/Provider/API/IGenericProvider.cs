using Mx.NET.SDK.Provider.Dtos.Common.QueryVm;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface IGenericProvider
    {
        /// <summary>
        /// Generic GET request to API
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/economics?extract=price')</param>
        /// <returns></returns>
        Task<TR> Get<TR>(string requestUri);

        /// <summary>
        /// Generic POST request to API
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/transactions')</param>
        /// <param name="requestContent">Request content object (e.g. TransactionRequestDto object)</param>
        /// <returns></returns>
        Task<TR> Post<TR>(string requestUri, object requestContent);
    }
}
