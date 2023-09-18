using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WalletConnectSharp.Core.Models.Pairing;
using Mx.NET.SDK.Wallet;
using Mx.NET.SDK.WalletConnect.Helper;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using UnityEngine;

namespace Mx.NET.SDK.WalletConnect
{
    public class WalletConnect : WalletConnectGeneric, IWalletConnect
    {
        public WalletConnect(Metadata metadata, string projectID, string chainID, string filePath = null)
            : base(metadata, projectID, chainID, filePath) { }

        public new async Task<SignableMessage> SignMessage(string message)
        {
            var signature = await base.SignMessage(message);
            var signableMessage = new SignableMessage()
            {
                Address = Core.Domain.Values.Address.FromBech32(Address),
                Message = message,
                Signature = signature
            };

            var isValid = signableMessage.VerifyMessage();
            if (!isValid)
                throw new Exception("Message signature is invalid");
            else
                return signableMessage;
        }

        public async Task<TransactionRequestDto> Sign(TransactionRequest transactionRequest)
        {
            var requestData = transactionRequest.GetSignTransactionRequest();
            var signature = await Sign(requestData);

            var transaction = transactionRequest.GetTransactionRequest();
            transaction.Signature = signature;
            return transaction;
        }

        public async Task<TransactionRequestDto[]> MultiSign(TransactionRequest[] transactionsRequest)
        {
            var requestsData = transactionsRequest.GetSignTransactionsRequest();
            var responseData = await MultiSign(requestsData);

            var transactions = new List<TransactionRequestDto>();
            for (var i = 0; i < responseData.Length; i++)
            {
                var transactionRequestDto = transactionsRequest[i].GetTransactionRequest();
                Debug.Log(transactionsRequest[i].Guardian);
                transactionRequestDto.Signature = responseData[i].Signature;
                transactionRequestDto.Guardian = responseData[i].Guardian;
                transactionRequestDto.GuardianSignature = responseData[i].GuardianSignature;
                transactionRequestDto.Version = responseData[i].Version;
                transactionRequestDto.Options = responseData[i].Options;
                transactions.Add(transactionRequestDto);
            }

            return transactions.ToArray();
        }
    }
}
