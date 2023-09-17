using System.Numerics;
using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Provider.Dtos.API.Accounts
{
    public class AccountDto
    {
        public string Address { get; set; }
        public string Balance { get; set; }
        public ulong Nonce { get; set; }
        public long Shard { get; set; }
        public dynamic Assets { get; set; } //JSON data
        public string Code { get; set; }
        public string CodeHash { get; set; }
        public string RootHash { get; set; }
        public BigInteger TxCount { get; set; }
        public BigInteger ScrCount { get; set; }
        public string UserName { get; set; }
        public string DeveloperReward { get; set; }
        public string OwnerAddress { get; set; }
        public bool IsUpgradable { get; set; }
        public bool IsReadable { get; set; }
        public bool IsPayable { get; set; }
        public bool IsPayableBySmartContract { get; set; }
        public string DeployTxHash { get; set; }
        public long DeployedAt { get; set; }
        public ScamInfoDto ScamInfo { get; set; }
        public long? ActiveGuardianActivationEpoch { get; set; }
        public string ActiveGuardianAddress { get; set; }
        public string ActiveGuardianServiceUid { get; set; }
        public long? PendingGuardianActivationEpoch { get; set; }
        public string PendingGuardianAddress { get; set; }
        public string PendingGuardianServiceUid { get; set; }
        public bool? IsGuarded { get; set; }
    }
}
