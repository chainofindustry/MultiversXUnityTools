using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models.Ethereum
{
    public sealed class EthSignTransaction : EthGenericRequest<TransactionData>
    {
        //[JsonProperty("params")]
        //private TransactionData[] _parameters;

        //[JsonIgnore]
        //public TransactionData[] Parameters => _parameters;

        public EthSignTransaction(TransactionData[] transactionDatas) : base(
            ValidJsonRpcRequestMethods.EthSignTransaction,
            transactionDatas
        )
        {
            ////this.Method = "erd_sign";
            //this.Method = "erd_batch_sign";
            //this._parameters = transactionDatas;
        }
    }
}