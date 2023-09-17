using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using Mx.NET.SDK.Provider.Dtos.Gateway.Transactions;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.Gateway
{
    public interface ITransactionsProvider
    {
        /// <summary>
        /// This endpoint allows one to send a signed Transaction to the Blockchain.
        /// </summary>
        /// <param name="transactionRequest">The transaction payload</param>
        /// <returns>TxHash</returns>
        Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequest);

        /// <summary>
        /// This endpoint allows one to send a bulk of Transactions to the Blockchain.
        /// </summary>
        /// <param name="transactionsRequest">Array of transactions payload</param>
        /// <returns><see cref="MultipleTransactionsResponseDto"/></returns>
        Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequest);

        /// <summary>
        /// This endpoint allows one to estimate the cost of a transaction.
        /// </summary>
        /// <param name="transactionRequestDto">The transaction payload</param>
        /// <returns><see cref="TransactionCostDataDto"/></returns>
        Task<TransactionCostResponseDto> GetTransactionCost(TransactionRequestDto transactionRequestDto);

        /// <summary>
        /// This endpoint allows one to query the details of a Transaction.
        /// </summary>
        /// <param name="txHash">The transaction hash</param>
        /// <param name="withResults">Get Smart Contract results</param>
        /// <returns><see cref="TransactionDto"/></returns>
        Task<TransactionDto> GetTransaction(string txHash, bool withResults = false);

        /// <summary>
        /// This endpoint allows one to query the details of a Transaction.
        /// </summary>
        /// <param name="txHash">The transaction hash</param>
        /// <param name="withResults">Get Smart Contract results</param>
        /// <returns>Your custom Transaction object</returns>
        Task<Transaction> GetTransaction<Transaction>(string txHash, bool withResults = false);
    }
}
