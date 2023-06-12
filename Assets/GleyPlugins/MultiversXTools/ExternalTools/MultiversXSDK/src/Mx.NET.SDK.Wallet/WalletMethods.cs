using System;
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
        public static TransactionRequestDto SignTransaction(this WalletSigner signer, TransactionRequest transactionRequest)
        {
            var transactionRequestDto = transactionRequest.GetTransactionRequest();
            var json = JsonWrapper.Serialize(transactionRequestDto);
            var message = Encoding.UTF8.GetBytes(json);

            transactionRequestDto.Signature = signer.Sign(message);
            return transactionRequestDto;
        }

        public static TransactionRequestDto[] SignTransactions(this WalletSigner signer, TransactionRequest[] transactionsRequest)
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

        public static bool VerifySignature(this TransactionRequest transactionRequest, string signature)
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

        public static bool VerifyMessage(this SignableMessage signableMessage)
        {
            if (signableMessage.Address is null)
                throw new Exception("Address is not initialized");

            var verifier = WalletVerifier.FromAddress(signableMessage.Address);
            return verifier.Verify(signableMessage);
        }
    }
}
