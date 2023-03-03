using Erdcsharp.Domain;
using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Network.Models;

namespace MultiversXUnityTools
{
    [RpcMethod("multiversx_signTransaction")]
    [RpcRequestOptions(Clock.TEN_SECONDS, true, 1108)]
    [RpcResponseOptions(Clock.TEN_SECONDS, false, 1109)]
    public class ErdSignTransaction
    {
        public TransactionData transaction;
        public ErdSignTransaction(TransactionData transactionData)
        {
            transaction = transactionData;
        }
    }
}
