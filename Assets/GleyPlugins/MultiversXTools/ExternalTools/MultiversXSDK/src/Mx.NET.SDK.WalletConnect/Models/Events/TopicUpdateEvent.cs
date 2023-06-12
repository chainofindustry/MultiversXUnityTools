using Newtonsoft.Json;

namespace Mx.NET.SDK.WalletConnect.Models.Events
{
    public class TopicUpdateEvent
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }
    }
}
