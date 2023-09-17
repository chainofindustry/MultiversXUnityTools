using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Constants;

namespace MultiversX.UnityTools.Examples
{
    /// <summary>
    /// Used to declare the stats of the tokens you want to be able to transfer inside your app
    /// </summary>
    public class SupportedESDTTokens
    {
        public static ESDT USDC = ESDT.ESDT_TOKEN(ESDTTokenType.FungibleESDT, "USDC", "USDC-8d4068", 6);
        public static ESDT WEB = ESDT.ESDT_TOKEN(ESDTTokenType.FungibleESDT, "WEB", "WEB-5d08be", 18);
    }
}
