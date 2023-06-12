using Mx.NET.SDK.WalletConnect.Data;
using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace Mx.NET.SDK.WalletConnect.Models
{
    [RpcMethod("mvx_signTransactions")]
    public class SignTransactionsRequest
    {
        [JsonProperty("transactions")]
        public RequestData[] Transactions;
    }

    public class SignTransactionsResponse
    {
        public ResponseData[] Signatures { get; set; }
    }
}
