namespace Mx.NET.SDK.Provider.Dtos.API.Account
{
    public class AccountMetaESDTDto
    {
        public string Identifier { get; set; }
        public string Collection { get; set; }
        public string Attributes { get; set; }
        public ulong Nonce { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public bool IsWhitelistedStorage { get; set; }
        public string Balance { get; set; }
        public int Decimals { get; set; }
        public string Ticker { get; set; }
        public string Price { get; set; }
        public string ValueUSD { get; set; }
        public dynamic Assets { get; set; } //JSON data
    }
}
