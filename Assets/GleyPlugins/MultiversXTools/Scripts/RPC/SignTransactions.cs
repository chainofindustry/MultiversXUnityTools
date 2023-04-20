using WalletConnectSharp.Network.Models;

namespace MultiversXUnityTools
{
    [RpcMethod(MultiversXRpcMethods.SIGN_TRANSACTIONS)]
    public class SignTransactions
    {
        public TransactionData[] transactions;
        public SignTransactions(TransactionData[] transactionData)
        {
            transactions = transactionData;
        }
    }

    public class SignTransactionsResponse
    {
        public SignTransactionResponse[] Signatures { get; set; }
    }
}