namespace Mx.NET.SDK.Provider.Dtos.API.NFTs
{
    public class MetaESDTDto
    {
        public string Identifier { get; set; }
        public string Collection { get; set; }
        public long Timestamp { get; set; }
        public string Attributes { get; set; }
        public ulong Nonce { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public bool IsWhitelistedStorage { get; set; }
        public string Supply { get; set; }
        public int Decimals { get; set; }
        public string Ticker { get; set; }
        public dynamic Assets { get; set; } //JSON data
    }
}
