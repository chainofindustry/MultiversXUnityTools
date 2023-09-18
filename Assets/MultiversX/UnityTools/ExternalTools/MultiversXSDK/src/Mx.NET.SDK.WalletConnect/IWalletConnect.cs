using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using System.Threading.Tasks;

namespace Mx.NET.SDK.WalletConnect
{
    public interface IWalletConnect : IWalletConnectGeneric
    {
        /// <summary>
        /// Sign a message
        /// </summary>
        /// <param name="message">Message to be signed</param>
        /// <returns>Signed message</returns>
        new Task<SignableMessage> SignMessage(string message);

        /// <summary>
        /// Request to xPortal app to sign a transaction
        /// </summary>
        /// <param name="transactionRequest">Transaction Request</param>
        /// <returns>Transaction payload</returns>
        Task<TransactionRequestDto> Sign(TransactionRequest transactionRequest);

        /// <summary>
        /// Request to xPortal app to sign multiple transactions
        /// </summary>
        /// <param name="transactionsRequest">Transactions Request</param>
        /// <returns>Transactions payload</returns>
        Task<TransactionRequestDto[]> MultiSign(TransactionRequest[] transactionsRequest);
    }
}
