using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace MultiversXUnityTools
{
    [RpcMethod(MultiversXRpcMethods.SIGN_TRANSACTIONS)]
    [RpcRequestOptions(Clock.TEN_SECONDS, true, 1108)]
    [RpcResponseOptions(Clock.TEN_SECONDS, false, 1109)]
    public class SignTransactions
    {
        public TransactionData[] transactions;
        public SignTransactions(TransactionData[] transactionData)
        {
            transactions = transactionData;
        }
    }
}