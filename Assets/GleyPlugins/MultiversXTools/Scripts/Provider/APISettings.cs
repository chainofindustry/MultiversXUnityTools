using UnityEngine;

namespace MultiversXUnityTools
{
    /// <summary>
    /// Scriptable object to store the editor configuration
    /// </summary>
    public class APISettings : ScriptableObject
    {
        public string selectedAPIName;

        //xPortal display values
        public string appDescription = "You are using Chain of Industry test login";
        public string appIcon = "https://gleygames.com/blockchain/COI-Building-Avatar-192x192.jpg";
        public string appName = "Chain of Industry";
        public string appWebsite = "https://chainofindustry.com/";
        public string projectID = "39f3dc0a2c604ec9885799f9fc5feb7c";
        public string savePath = "/wc/sessionData.json";
    }
}
