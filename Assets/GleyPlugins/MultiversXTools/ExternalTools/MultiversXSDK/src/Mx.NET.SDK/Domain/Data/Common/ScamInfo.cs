using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Domain.Data.Common
{
    public class ScamInfo
    {
        public string Type { get; private set; }
        public string Info { get; private set; }

        private ScamInfo() { }

        public static ScamInfo From(ScamInfoDto scamInfo)
        {
            if (scamInfo == null) return null;

            return new ScamInfo()
            {
                Type = scamInfo.Type,
                Info = scamInfo.Info
            };
        }
    }
}
