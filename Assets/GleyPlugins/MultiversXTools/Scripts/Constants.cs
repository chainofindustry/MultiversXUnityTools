namespace MultiversXUnityTools
{
    public class Constants
    {
        public const string CUSTOM_BRIDGE_URL = "https://f.bridge.walletconnect.org";

        public const string API_SETTINGS_DATA = "APISettingsData";
        public const string CONNECTION_MANAGER_OBJECT = "MultiversXConnectionManager";
        public const string NAMESPACE_NAME = "MultiversXUnityTools";

        public const string APIS_FOLDER = "Resources/APIs";
        public const string DEFAULT_APIS_FOLDER = "Scripts/Provider/DefaultAPIs";
        public const string PROVIDER_FOLDER = "Scripts/Provider";
        public const string RESOURCES_FOLDER = "Resources";

        //Bypass the CORS error on web when downloading NFTs see docs for more details
#if UNITY_WEBGL
        public const string CORSFixUrl = "https://gleygames.com/getImage.php?imageUrl=";
#else
        public const string CORSFixUrl = null;
#endif
    }
}