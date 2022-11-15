using Newtonsoft.Json;
using WalletConnectSharp.Core.Models;

namespace MultiversXUnityTools
{
    public sealed class ErdBatchSignTransaction : JsonRpcRequest
    {
        [JsonProperty("params")]
        private readonly TransactionData[] _parameters;

        [JsonIgnore]
        public TransactionData[] Parameters => _parameters;

        public ErdBatchSignTransaction(TransactionData[] transactionDatas) : base(
            ValidJsonRpcRequestMethods.ErdBatchSign
        )
        {
            _parameters = transactionDatas;
        }
    }
}