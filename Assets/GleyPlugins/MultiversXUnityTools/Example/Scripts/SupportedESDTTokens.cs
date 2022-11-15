using Erdcsharp.Domain;

namespace MultiversXUnityTools
{
    /// <summary>
    /// Used to declare the stats of the tokens you want to be able to transfer inside your app
    /// </summary>
    public class SupportedESDTTokens
    {
        public static Token USDC = new Token("USDC", "USDC-8d4068", 6);
        public static Token WEB = new Token("WEB", "WEB-5d08be", 18);
    }
}
