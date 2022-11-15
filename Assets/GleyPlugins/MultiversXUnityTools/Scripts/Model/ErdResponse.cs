using Newtonsoft.Json;
using WalletConnectSharp.Core.Models;

namespace MultiversXUnityTools
{
    public class ErdResponse : JsonRpcResponse
    {
        [JsonProperty]
        public Result result { get; set; }

        [JsonIgnore]
        public string Result => result.signature;
    }

    public class Result
    {
        public string signature { get; set; }
        public string[] accounts { get; set; }
        public bool approved { get; set; }
        public string chainId { get; set; }
        public string peerId { get; set; }
        public Peermeta peerMeta { get; set; }
    }

    public class Peermeta
    {
        public string description { get; set; }
        public object[] icons { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

}