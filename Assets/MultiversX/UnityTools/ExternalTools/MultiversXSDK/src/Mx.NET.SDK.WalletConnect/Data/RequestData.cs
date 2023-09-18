namespace Mx.NET.SDK.WalletConnect.Data
{
    public class RequestData
    {
        public string value { get; set; }
        public string chainID { get; set; }
        public string data { get; set; }
        public string sender { get; set; }
        public long gasLimit { get; set; }
        public long gasPrice { get; set; }
        public ulong nonce { get; set; }
        public string receiver { get; set; }
        public int version { get; set; }
        public int? options { get; set; }
        public string guardian { get; set; }
        public string guardianSignature { get; set; }
    }
}
