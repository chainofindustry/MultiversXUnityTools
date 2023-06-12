using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Provider.Dtos.Gateway.Network;
using Mx.NET.SDK.Provider.Dtos.Gateway.Query;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.Gateway
{
    public interface IGatewayProvider
    {
        /// <summary>
        /// Generic GET request to Gateway
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/economics?extract=price')</param>
        /// <returns></returns>
        Task<TR> GetGW<TR>(string requestUri);

        /// <summary>
        /// Generic POST request to Gateway
        /// </summary>
        /// <typeparam name="TR">Custom return object</typeparam>
        /// <param name="requestUri">Request endpoint (e.g. '/transactions')</param>
        /// <param name="requestContent">Request content object (e.g. TransactionRequestDto object)</param>
        /// <returns></returns>
        Task<TR> PostGW<TR>(string requestUri, object requestContent);

        /// <summary>
        /// This endpoint allows one to query basic details about the configuration of the Network.
        /// </summary>
        /// <returns><see cref="GatewayNetworkConfigDataDto"/></returns>
        Task<GatewayNetworkConfigDataDto> GetGatewayNetworkConfig();

        /// <summary>
        /// This endpoint allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output).
        /// </summary>
        /// <param name="queryVmRequestDto"></param>
        /// <returns><see cref="QueryVmResponseDto"/></returns>
        Task<QueryVmResponseDto> QueryVm(QueryVmRequestDto queryVmRequestDto);

        /// <summary>
        /// This endpoint allows one to send a signed Transaction to the Blockchain.
        /// </summary>
        /// <param name="transactionRequest">The transaction payload</param>
        /// <returns>TxHash</returns>
        Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequest);

        /// <summary>
        /// This endpoint allows one to send multiple signed Transactions to the Blockchain.
        /// </summary>
        /// <param name="transactionsRequest">Array of transactions payload</param>
        /// <returns><see cref="MultipleTransactionsResponseDto"/></returns>
        Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequest);
    }
}
