using Newtonsoft.Json;
using WalletConnectSharp.Sign.Models.Engine.Methods;

namespace Mx.NET.SDK.WalletConnect.Models.Events
{
    public class SessionUpdateEvent
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("params")]
        public dynamic Params { get; set; }
    }
}
