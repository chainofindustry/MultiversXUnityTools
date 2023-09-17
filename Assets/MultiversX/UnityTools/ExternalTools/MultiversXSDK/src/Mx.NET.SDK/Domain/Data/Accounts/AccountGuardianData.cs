using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Provider.Dtos.Gateway.Addresses;

namespace Mx.NET.SDK.Domain.Data.Accounts
{
    public class AccountGuardianData
    {
        public BlockInfo BlockInfo { get; set; }
        public GuardianData GuardianData { get; set; }

        private AccountGuardianData() { }

        public static AccountGuardianData From(AddressGuardianDataDto guardianData)
        {
            return new AccountGuardianData()
            {
                BlockInfo = BlockInfo.From(guardianData.BlockInfo),
                GuardianData = GuardianData.From(guardianData.GuardianData)
            };
        }
    }
}
