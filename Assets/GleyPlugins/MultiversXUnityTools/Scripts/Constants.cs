namespace MultiversXUnityTools
{
    public class Constants
    {
        public const string ApiSettingsData = "APISettingsData";
        public const string ConnectionManagerObject = "MultiversXConnectionManager";
        public const string customBridgeUrl = "https://f.bridge.walletconnect.org";

        public const string APIsFolder = "GleyPlugins/MultiversXUnityTools/Scripts/Provider/APIs";
        public const string DefaultAPIsFolder = "GleyPlugins/MultiversXUnityTools/Scripts/Provider/DefaultAPIs";
        public const string providerFolder = "GleyPlugins/MultiversXUnityTools/Scripts/Provider";
        public const string resourcesFolder = "Assets/GleyPlugins/MultiversXUnityTools/Resources";
        public const string namespaceName = "MultiversXUnityTools";


#if UNITY_WEBGL
        public const string CORSFixUrl = "https://gleygames.com/getImage.php?imageUrl=";
#else
        public const string CORSFixUrl = null;
#endif
    }
}