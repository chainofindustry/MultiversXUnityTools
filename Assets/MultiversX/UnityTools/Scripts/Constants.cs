namespace MultiversX.UnityTools
{
    public class Constants
    {
        public const string FOLDER_NAME = "UnityTools";
        public const string PARENT_FOLDER = "MultiversX";

        public const string APP_SETTINGS_DATA = "AppSettingsData";
        public const string CONNECTION_MANAGER_OBJECT = "MultiversXConnectionManager";

        public const string RESOURCES_FOLDER = "Resources";

        //Bypass the CORS error on web when downloading NFTs see docs for more details
#if UNITY_WEBGL
        public const string CORSFixUrl = "";
#else
        public const string CORSFixUrl = null;
#endif
    }
}