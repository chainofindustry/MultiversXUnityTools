namespace Mx.NET.SDK.Provider.Dtos.Common.Transactions
{
    public class TransactionRequestDto
    {
        public ulong Nonce { get; set; }
        public string Value { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public long GasPrice { get; set; }
        public long GasLimit { get; set; }
        public string Data { get; set; }
        public string ChainID { get; set; }
        public int Version { get; set; }
        public int? Options { get; set; }
        public string Guardian { get; set; }
        public string Signature { get; set; }
        public string GuardianSignature { get; set; }
    }
}
