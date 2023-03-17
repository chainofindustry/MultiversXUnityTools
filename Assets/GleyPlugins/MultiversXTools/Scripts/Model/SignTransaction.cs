using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace MultiversXUnityTools
{
    [RpcMethod(MultiversXRpcMethods.SIGN_TRANSACTION)]
    //[RpcRequestOptions(Clock.TEN_SECONDS, true, 1108)]
    //[RpcResponseOptions(Clock.TEN_SECONDS, false, 1109)]
    public class SignTransaction
    {
        public TransactionData transaction;
        public SignTransaction(TransactionData transactionData)
        {
            transaction = transactionData;
        }
    }
}
