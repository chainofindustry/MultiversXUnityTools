using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.Gateway.Addresses;

namespace Mx.NET.SDK.Domain.Data.Common
{
    public class BlockInfo
    {
        public string Hash { get; set; }
        public ulong Nonce { get; set; }
        public string RootHash { get; set; }

        private BlockInfo() { }

        public static BlockInfo From(BlockInfoDto blockInfo)
        {
            return new BlockInfo()
            {
                Hash = blockInfo.Hash,
                Nonce = blockInfo.Nonce,
                RootHash = blockInfo.RootHash
            };
        }
    }

    public class GuardianData
    {
        public ActiveGuardian ActiveGuardian { get; set; }
        public bool? Guarded { get; set; }
        public PendingGuardian PendingGuardian { get; set; }

        private GuardianData() { }

        public static GuardianData From(GuardianDataDto guardianData)
        {
            if (guardianData == null) return null;

            return new GuardianData()
            {
                ActiveGuardian = ActiveGuardian.From(guardianData.ActiveGuardian),
                Guarded = guardianData.Guarded,
                PendingGuardian = PendingGuardian.From(guardianData.PendingGuardian)
            };
        }
    }

    public class ActiveGuardian
    {
        public long ActivationEpoch { get; set; }
        public Address Address { get; set; }
        public string ServiceUID { get; set; }

        private ActiveGuardian() { }

        public static ActiveGuardian From(ActiveGuardianDto activeGuardian)
        {
            if (activeGuardian == null) return null;

            return new ActiveGuardian()
            {
                ActivationEpoch = activeGuardian.ActivationEpoch ?? 0,
                Address = Address.FromBech32(activeGuardian.Address),
                ServiceUID = activeGuardian.ServiceUID
            };
        }
    }

    public class PendingGuardian
    {
        public long ActivationEpoch { get; set; }
        public Address Address { get; set; }
        public string ServiceUID { get; set; }

        private PendingGuardian() { }

        public static PendingGuardian From(PendingGuardianDto pendingGuardian)
        {
            if (pendingGuardian == null) return null;

            return new PendingGuardian()
            {
                ActivationEpoch = pendingGuardian.ActivationEpoch ?? 0,
                Address = Address.FromBech32(pendingGuardian.Address),
                ServiceUID = pendingGuardian.ServiceUID
            };
        }
    }
}
