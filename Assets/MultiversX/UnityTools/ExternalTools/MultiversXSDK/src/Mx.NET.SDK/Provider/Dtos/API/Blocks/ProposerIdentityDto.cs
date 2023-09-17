namespace Mx.NET.SDK.Provider.Dtos.API.Blocks
{
    public class ProposerIdentityDto
    {
        public string Identity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Location { get; set; }
        public double? Score { get; set; }
        public int? Validators { get; set; }
        public string Stake { get; set; }
        public string TopUp { get; set; }
        public string Locked { get; set; }
        public dynamic Distribution { get; set; }
        public string[] Providers { get; set; }
        public double? StakePercent { get; set; }
        public double? Apr { get; set; }
        public int? Rank { get; set; }
    }
}
