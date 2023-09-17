using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Provider.Dtos.API.Accounts
{
    public class AccountContractDto
    {
        public string Address { get; set; }
        public string DeployTxHash { get; set; }
        public long Timestamp { get; set; }
        public AssetsDto Assets { get; set; }
    }
}
