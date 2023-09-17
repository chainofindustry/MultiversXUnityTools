using Mx.NET.SDK.Provider.Dtos.API.Blocks;

namespace Mx.NET.SDK.Domain.Data.Common
{
    /// <summary>
    /// Proposer Identity for Block
    /// </summary>
    public class ProposerIdentity
    {
        public string Identity { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Avatar { get; private set; }
        public string Website { get; private set; }
        public string Twitter { get; private set; }
        public string Location { get; private set; }
        public double Score { get; private set; }
        public int Validators { get; private set; }
        public string Stake { get; private set; }
        public string TopUp { get; private set; }
        public string Locked { get; private set; }
        public dynamic Distribution { get; private set; }
        public string[] Providers { get; private set; }
        public double StakePercent { get; private set; }
        public double Apr { get; private set; }
        public int Rank { get; private set; }

        private ProposerIdentity() { }

        public static ProposerIdentity From(ProposerIdentityDto proposerIdentity)
        {
            if (proposerIdentity == null) return null;

            return new ProposerIdentity()
            {
                Identity = proposerIdentity.Identity,
                Name = proposerIdentity.Name,
                Description = proposerIdentity.Description,
                Avatar = proposerIdentity.Avatar,
                Website = proposerIdentity.Website,
                Twitter = proposerIdentity.Twitter,
                Location = proposerIdentity.Location,
                Score = proposerIdentity.Score ?? 0,
                Validators = proposerIdentity.Validators ?? 0,
                Stake = proposerIdentity.Stake,
                TopUp = proposerIdentity.TopUp,
                Locked = proposerIdentity.Locked,
                Distribution = proposerIdentity.Distribution,
                Providers = proposerIdentity.Providers,
                StakePercent = proposerIdentity.StakePercent ?? 0,
                Apr = proposerIdentity.Apr ?? 0,
                Rank = proposerIdentity.Rank ?? 0
            };
        }
    }
}
