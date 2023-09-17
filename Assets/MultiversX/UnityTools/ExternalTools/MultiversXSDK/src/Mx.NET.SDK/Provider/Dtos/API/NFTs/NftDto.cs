using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Provider.Dtos.API.NFTs
{
    public class NFTDto
    {
        public string Identifier { get; set; }
        public string Collection { get; set; }
        public long TimeStamp { get; set; } = 0;
        public string Attributes { get; set; }
        public ulong Nonce { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public float Royalties { get; set; }
        public string[] URIs { get; set; }
        public string URL { get; set; }
        public dynamic[] Media { get; set; } //JSON data
        public bool IsWhitelistedStorage { get; set; }
        public string[] Tags { get; set; }
        public dynamic Metadata { get; set; } //JSON data
        public string Owner { get; set; }
        public string Supply { get; set; } = "1";
        public string Ticker { get; set; }
        public ScamInfoDto ScamInfo { get; set; }
        public bool IsNSFW { get; set; }
        public dynamic Assets { get; set; }
    }
}
