namespace Mx.NET.SDK.Provider.Dtos.API.Account
{
    public class AccountHistoryDto
    {
        public string Address { get; set; }
        public string Balance { get; set; }
        public long Timestamp { get; set; }
        public bool? IsSender { get; set; }
    }
}
