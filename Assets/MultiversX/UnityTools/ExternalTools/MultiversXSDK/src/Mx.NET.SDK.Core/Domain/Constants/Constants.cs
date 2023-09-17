namespace Mx.NET.SDK.Core.Domain.Constants
{
    public static class Constants
    {
        public const string ArwenVirtualMachine = "0500";

        /// <summary>
        /// Human-Readable Part
        /// </summary>
        public const string Hrp = "erd";

        /// <summary>
        /// eGold ticker
        /// </summary>
        public const string EGLD = "EGLD";

        /// <summary>
        /// ESDT Smart Contract bech32 address
        /// </summary>
        public const string ESDT_SMART_CONTRACT = "erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u";
    }

    public static class ESDTTokenType
    {
        public const string EGLD = "egld";
        public const string FungibleESDT = "FungibleESDT";
        public const string NonFungibleESDT = "NonFungibleESDT";
        public const string SemiFungibleESDT = "SemiFungibleESDT";
        public const string MetaESDT = "MetaESDT";
    }
}
