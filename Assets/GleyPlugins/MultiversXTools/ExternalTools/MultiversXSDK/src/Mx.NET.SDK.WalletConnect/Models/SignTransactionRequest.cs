using Mx.NET.SDK.WalletConnect.Data;
using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace Mx.NET.SDK.WalletConnect.Models
{
    [RpcMethod("mvx_signTransaction")]
    public class SignTransactionRequest
    {
        [JsonProperty("transaction")]
        public RequestData Transaction;
    }

    public class SignTransactionResponse
    {
        public string Signature { get; set; }
    }
}
