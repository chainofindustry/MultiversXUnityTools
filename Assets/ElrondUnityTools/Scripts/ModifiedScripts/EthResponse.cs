using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models.Ethereum
{
    public class EthResponse : JsonRpcResponse
    {
        [JsonProperty]
        public Result result { get; set; }

        [JsonIgnore]
        public string Result => result.signature;
    }

    public class Result
    {
        public string signature { get; set; }
    }

}