using UnityEngine;
using Network = Mx.NET.SDK.Configuration.Network;

namespace MultiversX.UnityTools
{
    /// <summary>
    /// Scriptable object to store the editor configuration
    /// </summary>
    public class AppSettings : ScriptableObject
    {
        public Network selectedNetwork;

        //xPortal display values
        public string appDescription = "Description";
        public string appIcon = "https://LinkToYourIcon.jpg";
        public string appName = "App Name";
        public string appWebsite = "https://multiversx.com/";
        
        //project settings
        public string projectID = "39f3dc0a2c604ec9885799f9fc5feb7c";
        //will be saved in Application.PersistentDataPath+savePath
        public string savePath = "/wc/sessionData.json";
    }
}
