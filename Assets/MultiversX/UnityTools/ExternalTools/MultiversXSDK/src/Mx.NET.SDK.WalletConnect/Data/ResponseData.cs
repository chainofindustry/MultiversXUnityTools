namespace Mx.NET.SDK.WalletConnect.Data
{
    public class ResponseData
    {
        public string Signature { get; set; }
        public string Guardian { get; set; }
        public string GuardianSignature { get; set; }
        public int Version { get; set; }
        public int Options { get; set; }
    }
}
