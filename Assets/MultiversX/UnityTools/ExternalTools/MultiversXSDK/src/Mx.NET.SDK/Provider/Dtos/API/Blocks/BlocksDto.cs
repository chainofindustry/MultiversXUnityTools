namespace Mx.NET.SDK.Provider.Dtos.API.Blocks
{
    public class BlocksDto
    {
        public string Hash { get; set; }
        public int? Epoch { get; set; }
        public ulong? Nonce { get; set; }
        public string PrevHash { get; set; }
        public string Proposer { get; set; }
        public ProposerIdentityDto ProposerIdentity { get; set; }
        public string PubKeyBitmap { get; set; }
        public ulong? Round { get; set; }
        public int? Shard { get; set; }
        public int? Size { get; set; }
        public int? SizeTxs { get; set; }
        public string StateRootHash { get; set; }
        public long? Timestamp { get; set; }
        public long? TxCount { get; set; }
        public ulong? GasConsumed { get; set; }
        public ulong? GasRefunded { get; set; }
        public ulong? GasPenalized { get; set; }
        public ulong? MaxGasLimit { get; set; }
    }
}
