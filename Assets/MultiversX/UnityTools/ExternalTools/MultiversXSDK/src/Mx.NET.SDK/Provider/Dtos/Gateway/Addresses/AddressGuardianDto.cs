namespace Mx.NET.SDK.Provider.Dtos.Gateway.Addresses
{
    public class AddressGuardianDataDto
    {
        public BlockInfoDto BlockInfo { get; set; }
        public GuardianDataDto GuardianData { get; set; }
    }

    public class BlockInfoDto
    {
        public string Hash { get; set; }
        public ulong Nonce { get; set; }
        public string RootHash { get; set; }
    }

    public class GuardianDataDto
    {
        public ActiveGuardianDto ActiveGuardian { get; set; }
        public bool? Guarded { get; set; }
        public PendingGuardianDto PendingGuardian { get; set; }
    }

    public class ActiveGuardianDto
    {
        public long? ActivationEpoch { get; set; }
        public string Address { get; set; }
        public string ServiceUID { get; set; }
    }

    public class PendingGuardianDto
    {
        public long? ActivationEpoch { get; set; }
        public string Address { get; set; }
        public string ServiceUID { get; set; }
    }
}
