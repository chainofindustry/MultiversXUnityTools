namespace Mx.NET.SDK.Provider.Dtos.API.Accounts
{
    public class AccountTokenDto
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Owner { get; set; }
        public int Decimals { get; set; }
        public bool IsPaused { get; set; }
        public dynamic Assets { get; set; } //JSON data
        public string Price { get; set; }
        public string Balance { get; set; }
        public string ValueUSD { get; set; }
    }
}
