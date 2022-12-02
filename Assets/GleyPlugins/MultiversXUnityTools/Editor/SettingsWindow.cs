using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MultiversXUnityTools
{
    public class SettingsWindow : EditorWindow
    {
        private List<API> supportedAPIs;
        private API selectedAPI;
        private APISettings apiSettings;
        private Vector2 scrollPosition = Vector2.zero;
        private ContactButton[] contactButtons;
        private IconReferences iconReferences;
        private string apiName;
        private string baseAddress;
        private bool addAPI;
        private bool showAvailableAPIs;


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
        [MenuItem("Tools/MultiversX Unity Tools/Settings Window", false, 50)]
        private static void Init()
        {
            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow));
            window.titleContent = new GUIContent("MultiversX Tools - v.0.5.2");
            window.minSize = new Vector2(620, 520);
            window.Show();
        }


        //load settings files
        private void OnEnable()
        {
            if (iconReferences == null)
            {
                LoadIcons();
            }

            apiSettings = Resources.Load<APISettings>(Constants.API_SETTINGS_DATA);
            if (apiSettings == null)
            {
                CreateAPISettings();
                apiSettings = Resources.Load<APISettings>(Constants.API_SETTINGS_DATA);
            }

            supportedAPIs = new List<API>();
            DirectoryInfo dir = new DirectoryInfo($"{Application.dataPath}/{Constants.APIS_FOLDER}");
            if (!dir.Exists)
            {
                Debug.Log("No custom APIs found, loading default APIs");
                FileUtil.CopyFileOrDirectory($"{Application.dataPath}/{Constants.DEFAULT_APIS_FOLDER}", $"{Application.dataPath}/{Constants.APIS_FOLDER}");
                AssetDatabase.Refresh();
                dir = new DirectoryInfo($"{Application.dataPath}/{Constants.APIS_FOLDER}");
            }

            foreach (FileInfo file in dir.GetFiles("*.json"))
            {
                StreamReader reader = new StreamReader(file.FullName);
                supportedAPIs.Add(JsonConvert.DeserializeObject<API>(reader.ReadToEnd()));
                if (supportedAPIs[supportedAPIs.Count - 1].apiName == apiSettings.selectedAPIName)
                {
                    selectedAPI = supportedAPIs[supportedAPIs.Count - 1];
                }
                reader.Close();
            }

            if (selectedAPI == null)
            {
                if (supportedAPIs.Count != 0)
                {
                    selectedAPI = supportedAPIs[0];
                }
            }

            contactButtons = new ContactButton[]
            {
                new ContactButton(new GUIContent(" Documentation", iconReferences.websiteIcon),"https://github.com/chainofindustry/MultiversXUnityTools/wiki"),
                new ContactButton(new GUIContent(" Youtube", iconReferences.youtubeIcon),"https://www.youtube.com/channel/UCmvJB1_IobMjYKCNBtuZBog"),
                new ContactButton(new GUIContent(" Twitter", iconReferences.twitterIcon),"https://twitter.com/XUnityTools"),
                //new ContactButton(new GUIContent(" Discord", iconReferences.discordIcon),"https://discord.gg/7eSvKKW"),            
            };
        }
        

        //Load Icon images
        void LoadIcons()
        {
            Object assetToLoad = AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/MultiversXUnityTools/Editor/IconReferences.asset", typeof(IconReferences));
            iconReferences = (IconReferences)assetToLoad;
        }

        //draw the Settings Window
        private void OnGUI()
        {
            if (selectedAPI == null)
            {
                Debug.LogWarning("No API found, please reimport the plugin");
                return;
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height - 50));
            EditorGUILayout.Space();

            //input details for Maiar
            #region MaiarConfiguration
            EditorGUILayout.LabelField("Maiar configuration ", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            apiSettings.appDescription = EditorGUILayout.TextField("App Description", apiSettings.appDescription);
            apiSettings.appIcon = EditorGUILayout.TextField("App Icon", apiSettings.appIcon);
            apiSettings.appName = EditorGUILayout.TextField("App Name", apiSettings.appName);
            apiSettings.appWebsite = EditorGUILayout.TextField("App Website", apiSettings.appWebsite);
            EditorGUILayout.Space();
            #endregion

            //display selected API
            #region SelectedAPI
            EditorGUILayout.LabelField("API configuration ", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Selected API Name: {selectedAPI.apiName}", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
            #endregion

            //Manage API endpoints 
            #region Endpoints
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (selectedAPI.endpoints != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Endpoint Name", GUILayout.Width(150));
                EditorGUILayout.LabelField("Base Address");
                EditorGUILayout.LabelField("Resource Name");
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < selectedAPI.endpoints.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    selectedAPI.endpoints[i].name = EditorGUILayout.TextField(selectedAPI.endpoints[i].name, GUILayout.Width(150));
                    if (selectedAPI.endpoints[i].name != null)
                    {
                        selectedAPI.endpoints[i].name = Regex.Replace(selectedAPI.endpoints[i].name, @"^[\d-]*\s*", "");
                        selectedAPI.endpoints[i].name = Regex.Replace(selectedAPI.endpoints[i].name, "[^a-zA-Z0-9._]", "");
                        selectedAPI.endpoints[i].name = selectedAPI.endpoints[i].name.Replace(" ", "");
                        selectedAPI.endpoints[i].name = selectedAPI.endpoints[i].name.Trim();
                    }
                    selectedAPI.endpoints[i].baseAddress = EditorGUILayout.TextField(selectedAPI.endpoints[i].baseAddress);
                    selectedAPI.endpoints[i].resourceName = EditorGUILayout.TextField(selectedAPI.endpoints[i].resourceName);
                    if (GUILayout.Button("Delete"))
                    {
                        selectedAPI.endpoints.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Add New Endpoint"))
            {
                if (selectedAPI.endpoints == null)
                {
                    selectedAPI.endpoints = new List<APIEndpoint>();
                }
                selectedAPI.endpoints.Add(new APIEndpoint("", selectedAPI.baseAddress));
            }
            #endregion

            //Select default API
            #region AvailableAPIs
            EditorGUILayout.LabelField("Available APIs: ", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (showAvailableAPIs)
            {
                if (GUILayout.Button("Hide APIs"))
                {
                    showAvailableAPIs = false;
                }
            }
            else
            {
                if (GUILayout.Button("Show APIs"))
                {
                    showAvailableAPIs = true;
                }
            }
            if (showAvailableAPIs)
            {
                for (int i = 0; i < supportedAPIs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"API name: {supportedAPIs[i].apiName}");
                    EditorGUILayout.LabelField($"Base address: {supportedAPIs[i].baseAddress}");
                    string buttonText = "Select";
                    if (supportedAPIs[i] == selectedAPI)
                    {
                        buttonText = "Selected";
                    }
                    if (GUILayout.Button(buttonText))
                    {
                        selectedAPI = supportedAPIs[i];
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        supportedAPIs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            #endregion

            //Manage APIs
            #region AddNewAPI
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (addAPI == false)
            {
                if (GUILayout.Button("Add new API"))
                {
                    addAPI = true;
                }
            }
            if (addAPI == true)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("API name:", GUILayout.Width(60));
                apiName = EditorGUILayout.TextField(apiName, GUILayout.Width(150));
                if (apiName != null)
                {
                    apiName = Regex.Replace(apiName, @"^[\d-]*\s*", "");
                    apiName = Regex.Replace(apiName, "[^a-zA-Z0-9._]", "");
                    apiName = apiName.Replace(" ", "");
                    apiName = apiName.Trim();
                }
                EditorGUILayout.LabelField("Base address: ", GUILayout.Width(80));
                baseAddress = EditorGUILayout.TextField(baseAddress);
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Save API"))
                {
                    AddAPI();
                }
                if (GUILayout.Button("Cancel"))
                {
                    addAPI = false;
                }
            }
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.EndScrollView();

            //Support
            #region Support
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < contactButtons.Length; i++)
            {
                if (GUILayout.Button(contactButtons[i].guiContent))
                {
                    Application.OpenURL(contactButtons[i].url);
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion

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
            CreateSupportedAPIEnum();
            CreateEndPointEnum();
            DeleteOldAPIFiles();
            for (int i = 0; i < supportedAPIs.Count; i++)
            {
                SaveJson(supportedAPIs[i]);
            }
            apiSettings.selectedAPIName = selectedAPI.apiName;
            EditorUtility.SetDirty(apiSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Add new API to API list
        /// </summary>
        private void AddAPI()
        {
            addAPI = false;
            selectedAPI = new API(apiName, baseAddress);
            supportedAPIs.Add(selectedAPI);
        }


        /// <summary>
        /// Clear old API files-> every time all files are regenerated when Save is press
        /// </summary>
        private void DeleteOldAPIFiles()
        {
            DirectoryInfo dir = new DirectoryInfo($"{Application.dataPath}/{Constants.APIS_FOLDER}");

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }


        /// <summary>
        /// Generate SupportedAPIs -> An enum to help access the APIs at runtime
        /// </summary>
        private void CreateSupportedAPIEnum()
        {
            string text =
            $"namespace {Constants.NAMESPACE_NAME}\n" +
            "{\n" +
            "\tpublic enum SupportedAPIs\n" +
            "\t{\n";
            for (int i = 0; i < supportedAPIs.Count; i++)
            {
                text += "\t\t" + supportedAPIs[i].apiName + ",\n";
                SaveJson(supportedAPIs[i]);
            }
            text += "\t}\n}";
            File.WriteAllText($"{Application.dataPath}/{Constants.PROVIDER_FOLDER}/SupportedAPIs.cs", text);
        }


        /// <summary>
        /// Generate EndpointNames enum -> Used to easy access the endpoints at runtime
        /// </summary>
        private void CreateEndPointEnum()
        {
            List<string> enumElements = new List<string>();
            List<string> debugValues = new List<string>();
            for (int i = 0; i < supportedAPIs.Count; i++)
            {
                if (supportedAPIs[i].endpoints != null)
                {
                    for (int j = 0; j < supportedAPIs[i].endpoints.Count; j++)
                    {
                        if (!enumElements.Contains(supportedAPIs[i].endpoints[j].name))
                        {
                            enumElements.Add(supportedAPIs[i].endpoints[j].name);
                            debugValues.Add(supportedAPIs[i].apiName);
                        }
                    }
                }
            }
            //verify if all endpoints are present in all APIs
            for (int i = 0; i < supportedAPIs.Count; i++)
            {
                if (supportedAPIs[i].endpoints != null)
                {
                    for (int j = 0; j < enumElements.Count; j++)
                    {
                        if (supportedAPIs[i].endpoints.FirstOrDefault(p => p.name == enumElements[j]) == null)
                        {
                            Debug.LogWarning($"Endpoint {enumElements[j]} from API {debugValues[j]} was not fount in API {supportedAPIs[i].apiName}");
                        }
                    }
                }
            }

            //generate file
            string text =
            $"namespace {Constants.NAMESPACE_NAME}\n" +
            "{\n" +
            "\tpublic enum EndpointNames\n" +
            "\t{\n";
            for (int i = 0; i < enumElements.Count; i++)
            {
                text += "\t\t" + enumElements[i] + ",\n";
            }
            text += "\t}\n}";
            File.WriteAllText($"{Application.dataPath}/{Constants.PROVIDER_FOLDER}/EndpointNames.cs", text);
        }


        /// <summary>
        /// Save API configuration as a json file
        /// </summary>
        /// <param name="api"></param>
        private void SaveJson(API api)
        {
            string json = JsonConvert.SerializeObject(api);
            File.WriteAllText($"{Application.dataPath}/{Constants.APIS_FOLDER}/{api.apiName}.json", json);
        }


        /// <summary>
        /// Create Settings data to store the save values
        /// </summary>
        private void CreateAPISettings()
        {
            APISettings asset = CreateInstance<APISettings>();
            EditorUtilities.CreateFolder(Constants.RESOURCES_FOLDER);
            AssetDatabase.CreateAsset(asset, $"{Constants.RESOURCES_FOLDER}/{Constants.API_SETTINGS_DATA}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
