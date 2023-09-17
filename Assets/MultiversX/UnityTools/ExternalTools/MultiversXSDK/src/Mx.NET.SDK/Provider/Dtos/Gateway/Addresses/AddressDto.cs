namespace Mx.NET.SDK.Provider.Dtos.Gateway.Addresses
{
    public class AddressDataDto
    {
        public AccountDto Account { get; set; }
    }

    public class AccountDto
    {
        public string Address { get; set; }
        public ulong Nonce { get; set; }
        public string Balance { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string CodeHash { get; set; }
        public string RootHash { get; set; }
        public string CodeMetadata { get; set; }
        public string DeveloperReward { get; set; }
        public string OwnerAddress { get; set; }
    }
}
