using WalletConnectSharp.Network.Models;

namespace MultiversXUnityTools
{
    [RpcMethod(MultiversXRpcMethods.SIGN_TRANSACTION)]
    public class SignTransaction
    {
        public TransactionData transaction;
        public SignTransaction(TransactionData transactionData)
        {
            transaction = transactionData;
        }
    }

    public class SignTransactionResponse
    {
        public string Signature { get; set; }
    }
}
