namespace ElrondUnityTools
{
    public class Constants
    {
        public const string ApiSettingsData = "APISettingsData";
        public const string ConnectionManagerObject = "ElrondConnectionManager";
        public const string customBridgeUrl = "https://f.bridge.walletconnect.org";

        //ElrondNetwork setup
        public const Erdcsharp.Configuration.Network networkType = Erdcsharp.Configuration.Network.DevNet;

        //Maiar display values
        public const string appDescription = "You are using Chain of Industry test login";
        public const string appIcon = "https://gleygames.com/blockchain/COI-Building-Avatar-192x192.jpg";
        public const string appName = "Chain of Industry";
        public const string appWebsite = "https://chainofindustry.com/";

#if UNITY_WEBGL
        public const string CORSFixUrl = "https://gleygames.com/getImage.php?imageUrl=";
#else
        public const string CORSFixUrl = null;
#endif
    }
}