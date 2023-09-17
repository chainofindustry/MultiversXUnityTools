namespace Mx.NET.SDK.Provider.Dtos.API.Network
{
    public class NetworkStatsDto
    {
        public int Shards { get; set; }
        public long Blocks { get; set; }
        public long Accounts { get; set; }
        public long Transactions { get; set; }
        public long RefreshRate { get; set; }
        public long Epoch { get; set; }
        public long RoundsPassed { get; set; }
        public long RoundsPerEpoch { get; set; }
    }
}
