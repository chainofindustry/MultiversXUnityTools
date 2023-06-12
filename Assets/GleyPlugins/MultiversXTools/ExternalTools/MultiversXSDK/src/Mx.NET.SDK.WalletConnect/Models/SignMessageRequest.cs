using Newtonsoft.Json;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;

namespace Mx.NET.SDK.WalletConnect.Models
{
    [RpcMethod("mvx_signMessage")]
    public class SignMessageRequest
    {
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        public SignMessageRequest(string address, string message)
        {
            Address = address;
            Message = message;
        }
    }

    public class SignMessageResponse
    {
        public string Signature { get; set; }
    }
}
