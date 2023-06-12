namespace MultiversXUnityTools
{
    public class Constants
    {
        public const string FOLDER_NAME = "MultiversXTools";
        public const string PARENT_FOLDER = "GleyPlugins";

        public const string APP_SETTINGS_DATA = "AppSettingsData";
        public const string CONNECTION_MANAGER_OBJECT = "MultiversXConnectionManager";

        public const string RESOURCES_FOLDER = "Resources";

        //Bypass the CORS error on web when downloading NFTs see docs for more details
#if UNITY_WEBGL
        public const string CORSFixUrl = "https://gleygames.com/getImage.php?imageUrl=";
#else
        public const string CORSFixUrl = null;
#endif
    }
}