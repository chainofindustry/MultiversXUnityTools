using Newtonsoft.Json;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Network.Models;

namespace MultiversXUnityTools
{
    //construct single transaction for signing
    public sealed class ErdSignTransaction : JsonRpcRequest<TransactionData>
    {
        //[JsonProperty("params")]
        //private readonly TransactionData _parameters;

        //[JsonIgnore]
        //public TransactionData Parameters => _parameters;

        public ErdSignTransaction(TransactionData transactionData) : base(
            ValidJsonRpcRequestMethods.ErdSign, transactionData
        )
        {

        }
    }
}
