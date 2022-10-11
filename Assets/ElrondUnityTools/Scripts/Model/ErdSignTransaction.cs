using Newtonsoft.Json;
using WalletConnectSharp.Core.Models;

namespace ElrondUnityTools
{
    public sealed class ErdSignTransaction : JsonRpcRequest
    {
        [JsonProperty("params")]
        private readonly TransactionData _parameters;

        [JsonIgnore]
        public TransactionData Parameters => _parameters;

        public ErdSignTransaction(TransactionData transactionData) : base(
            ValidJsonRpcRequestMethods.ErdSign
        )
        {
            _parameters = transactionData;
        }
    }
}
