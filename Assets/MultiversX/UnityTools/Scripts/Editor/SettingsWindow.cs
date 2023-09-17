using System.IO;
using UnityEditor;
using UnityEngine;
using Network = Mx.NET.SDK.Configuration.Network;

namespace MultiversX.UnityTools
{
    public class SettingsWindow : EditorWindow
    {
        private static string rootFolder;
        private static string rootWithoutAssets;

        private AppSettings appSettings;
        private Vector2 scrollPosition = Vector2.zero;


        //For contact section
        struct ContactButton
        {
            public GUIContent guiContent;
            public string url;

            public ContactButton(GUIContent guiContent, string url)
            {
                this.guiContent = guiContent;
                this.url = url;
            }
        }

        //Open Settings Window
        [MenuItem("Tools/MultiversX/Unity Tools", false, 50)]
        private static void Init()
        {
            if (!LoadRootFolder())
                return;

            string path = $"{rootFolder}/Scripts/Version.txt";

            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<AssetVersion>(reader.ReadToEnd()).longVersion;

            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow));
            window.titleContent = new GUIContent("MultiversX Tools - v." + longVersion);
            window.minSize = new Vector2(520, 320);
            window.Show();
        }


        //load settings files
        private void OnEnable()
        {
            if (!LoadRootFolder())
                return;

            appSettings = Resources.Load<AppSettings>(Constants.APP_SETTINGS_DATA);
            if (appSettings == null)
            {
                CreateAPISettings();
                appSettings = Resources.Load<AppSettings>(Constants.APP_SETTINGS_DATA);
            }
        }


        static bool LoadRootFolder()
        {
            rootFolder = EditorUtilities.FindFolder(Constants.FOLDER_NAME, Constants.PARENT_FOLDER);
            if (rootFolder == null)
            {
                Debug.LogError($"Folder Not Found:'{Constants.PARENT_FOLDER}/{Constants.FOLDER_NAME}'");
                return false;
            }
            rootWithoutAssets = rootFolder.Substring(7, rootFolder.Length - 7);
            return true;
        }

        //draw the Settings Window
        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height - 50));
            EditorGUILayout.Space();

            //input details for xPortal
            #region xPortalConfiguration
            EditorGUILayout.LabelField("xPortal configuration ", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            appSettings.appDescription = EditorGUILayout.TextField("App Description", appSettings.appDescription);
            appSettings.appIcon = EditorGUILayout.TextField("App Icon", appSettings.appIcon);
            appSettings.appName = EditorGUILayout.TextField("App Name", appSettings.appName);
            appSettings.appWebsite = EditorGUILayout.TextField("App Website", appSettings.appWebsite);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Project Settings ", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            appSettings.selectedNetwork = (Network)EditorGUILayout.EnumPopup("Network", appSettings.selectedNetwork);
            appSettings.projectID = EditorGUILayout.TextField("Project ID", appSettings.projectID);
            appSettings.savePath = EditorGUILayout.TextField("Save path", appSettings.savePath);
            EditorGUILayout.Space();
            #endregion

            GUILayout.EndScrollView();

            //Save current settings
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
        }


        /// <summary>
        /// Create required files and save current configuration
        /// </summary>
        private void SaveSettings()
        {
            EditorUtility.SetDirty(appSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Save Success");
        }

        /// <summary>
        /// Create Settings data to store the save values
        /// </summary>
        private void CreateAPISettings()
        {
            AppSettings asset = CreateInstance<AppSettings>();
            string path = $"{rootFolder}/{Constants.RESOURCES_FOLDER}";
            EditorUtilities.CreateFolder(path);
            AssetDatabase.CreateAsset(asset, $"{path}/{Constants.APP_SETTINGS_DATA}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
