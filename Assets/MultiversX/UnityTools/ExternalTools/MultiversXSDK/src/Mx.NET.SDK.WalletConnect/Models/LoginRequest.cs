using Newtonsoft.Json;
using WalletConnectSharp.Network.Models;

namespace Mx.NET.SDK.WalletConnect.Models
{
    [RpcMethod("mvx_signNativeAuthToken")]
    public class LoginRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }

        public LoginRequest(string token, string address)
        {
            Token = token;
            Address = address;
        }
    }

    public class LoginResponse
    {
        public string Signature { get; set; }
    }
}
