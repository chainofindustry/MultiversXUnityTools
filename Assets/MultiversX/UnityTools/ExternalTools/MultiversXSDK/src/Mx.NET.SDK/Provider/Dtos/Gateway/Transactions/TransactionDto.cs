namespace Mx.NET.SDK.Provider.Dtos.Gateway.Transactions
{
    public class TransactionDto
    {
        public string Type { get; set; }
        public string Hash { get; set; }
        public long Nonce { get; set; }
        public long Round { get; set; }
        public long Epoch { get; set; }
        public string Value { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public long GasPrice { get; set; }
        public long GasLimit { get; set; }
        public long GasUsed { get; set; }
        public string Data { get; set; }
        public string Signature { get; set; }
        public long SourceShard { get; set; }
        public long DestinationShard { get; set; }
        public long BlockNonce { get; set; }
        public string BlockHash { get; set; }
        public long NotarizedAtSourceInMetaNonce { get; set; }
        public string NotarizedAtSourceInMetaHash { get; set; }
        public long NotarizedAtDestinationInMetaNonce { get; set; }
        public string NotarizedAtDestinationInMetaHash { get; set; }
        public string MiniblockType { get; set; }
        public string MiniblockHash { get; set; }
        public long HyperblockNonce { get; set; }
        public string HyperblockHash { get; set; }
        public long Timestamp { get; set; }
        public SmartContractResultDto[] SmartContractResults { get; set; }
        public dynamic Logs { get; set; } //property not implemented
        public string Status { get; set; }
        public string[] Tokens { get; set; }
        public string[] EsdtValues { get; set; }
        public string Operation { get; set; }
        public string Function { get; set; }
        public string InitiallyPaidFee { get; set; }
        public string Fee { get; set; }
        public string ChainId { get; set; }
        public long Version { get; set; }
        public long Options { get; set; }

    }

    public class SmartContractResultDto
    {
        public string Hash { get; set; }
        public long Nonce { get; set; }
        public long Value { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public string Data { get; set; }
        public string PrevTxHash { get; set; }
        public string OriginalTxHash { get; set; }
        public long GasLimit { get; set; }
        public long GasPrice { get; set; }
        public long CallType { get; set; }
        public dynamic Logs { get; set; } //property not implemented
        public string Operation { get; set; }
    }
}
