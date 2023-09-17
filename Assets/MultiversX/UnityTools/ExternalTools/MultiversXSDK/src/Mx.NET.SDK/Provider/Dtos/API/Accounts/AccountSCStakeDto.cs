namespace Mx.NET.SDK.Provider.Dtos.API.Accounts
{
    public class AccountSCStakeDto
    {
        public string TotalStaked { get; set; }
        public UnstakedTokenDto[] UnstakedTokens { get; set; }
    }

    public class UnstakedTokenDto
    {
        public string Amount { get; set; }
        public long? Expires { get; set; }
        public long? Epochs { get; set; }
    }
}
