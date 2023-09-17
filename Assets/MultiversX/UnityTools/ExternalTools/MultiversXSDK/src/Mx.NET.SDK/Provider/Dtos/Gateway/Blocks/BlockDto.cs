using Mx.NET.SDK.Provider.Dtos.Gateway.Transactions;

namespace Mx.NET.SDK.Provider.Dtos.Gateway.Blocks
{
    public class BlockDataDto
    {
        public BlockDto Block { get; set; }
    }

    public class BlockDto
    {
        public long Nonce { get; set; }
        public long Round { get; set; }
        public string Hash { get; set; }
        public string PrevBlockHash { get; set; }
        public long Epoch { get; set; }
        public long Shard { get; set; }
        public long NumTxs { get; set; }
        public MiniBlockDto[] MiniBlocks { get; set; }
    }

    public class MiniBlockDto
    {
        public string Hash { get; set; }
        public string Type { get; set; }
        public long SourceShard { get; set; }
        public long DestinationShard { get; set; }
        public TransactionDto[] Transactions { get; set; }
    }

    /// <summary>
    /// Block
    /// </summary>
    /// <summary>
    /// Gateway block response
    /// </summary>
    public class InternalBlockResponseDto
    {
        public InternalBlockDto Block { get; set; }
    }

    public class InternalBlockDto
    {
        public InternalBlockHeaderDto Header { get; set; }
        public long ScheduledAccumulatedFees { get; set; }
        public long ScheduledDeveloperFees { get; set; }
        public string ScheduledRootHash { get; set; }
    }

    public class InternalBlockHeaderDto
    {
        public long Nonce { get; set; }
        public long Round { get; set; }
        public string RandSeed { get; set; }
        public string PrevRandSeed { get; set; }
        public string PrevHash { get; set; }
        public long Epoch { get; set; }
        public long ShardID { get; set; }

    }
}
