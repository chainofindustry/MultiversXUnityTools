using System.Collections.Generic;
using System.Text;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Wallet.Wallet;

namespace Mx.NET.SDK.Wallet
{
    public static class WalletMethods
    {
        public static TransactionRequestDto Sign(this TransactionRequest transactionRequest, Signer signer)
        {
            var transactionRequestDto = transactionRequest.GetTransactionRequest();
            var json = JsonWrapper.Serialize(transactionRequestDto);
            var message = Encoding.UTF8.GetBytes(json);

            transactionRequestDto.Signature = signer.Sign(message);
            return transactionRequestDto;
        }

        public static bool VerifySign(this TransactionRequest transactionRequest, string signature)
        {
            var transactionRequestDto = transactionRequest.GetTransactionRequest();
            var message = JsonWrapper.Serialize(transactionRequestDto);

            var verifier = WalletVerifier.FromAddress(transactionRequest.Sender);
            return verifier.VerifyRaw(new SignableMessage()
            {
                Message = message,
                Signature = signature
            });
        }

        public static TransactionRequestDto[] MultiSign(this TransactionRequest[] transactionsRequest, Signer signer)
        {
            var transactions = new List<TransactionRequestDto>();

            foreach (var transactionRequest in transactionsRequest)
            {
                var transactionRequestDto = transactionRequest.GetTransactionRequest();
                var json = JsonWrapper.Serialize(transactionRequestDto);
                var message = Encoding.UTF8.GetBytes(json);

                transactionRequestDto.Signature = signer.Sign(message);
                transactions.Add(transactionRequestDto);
            }

            return transactions.ToArray();
        }
    }
}
