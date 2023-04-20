using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Provider.Dtos.API.Collection
{
    public class CollectionDto
    {
        public string Collection { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Owner { get; set; }
        public long Timestamp { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanWipe { get; set; }
        public bool CanPause { get; set; }
        public bool CanTransferNFTCreateRole { get; set; }
        public bool CanChangeOwner { get; set; }
        public bool CanUpgrade { get; set; }
        public bool CanAddSpecialRoles { get; set; }
        public int Decimals { get; set; }
        public dynamic Assets { get; set; }
        public CollectionRolesDto[] Roles { get; set; }
        public ScamInfoDto ScamInfo { get; set; }
    }
}
