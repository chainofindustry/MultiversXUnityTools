using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface ITransactionsProvider
    {
        /// <summary>
        /// Returns the transactions satisfying the parameters
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns><see cref="TransactionDto"/></returns>
        Task<TransactionDto[]> GetTransactions(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the transactions satisfying the parameters
        /// </summary>
        /// <param name="size">Number of items to retrieve (max 10000)</param>
        /// <param name="from">Number of items to skip for the result set</param>
        /// <param name="parameters">Parameters for query</param>
        /// <returns>Array of your custom Transaction objects</returns>
        Task<Transaction[]> GetTransactions<Transaction>(int size = 100, int from = 0, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Returns the counter of transactions from blockchain.
        /// </summary>
        /// <param name="parameters">Parameters for query</param>
        /// <returns></returns>
        Task<string> GetTransactionsCount(Dictionary<string, string> parameters = null);

        /// <summary>
        /// This endpoint allows one to query the details of a Transaction.
        /// </summary>
        /// <param name="txHash">The transaction hash</param>
        /// <returns><see cref="TransactionDto"/></returns>
        Task<TransactionDto> GetTransaction(string txHash);

        /// <summary>
        /// This endpoint allows one to query the details of a Transaction.
        /// </summary>
        /// <param name="txHash">The transaction hash</param>
        /// <returns>Your custom Transaction object</returns>
        Task<Transaction> GetTransaction<Transaction>(string txHash);
    }
}
