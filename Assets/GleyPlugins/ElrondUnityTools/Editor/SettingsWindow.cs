using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ElrondUnityTools
{
    public class SettingsWindow : EditorWindow
    {
        string apiName;
        string baseAddress;
        bool addAPI;
        private Vector2 scrollPosition = Vector2.zero;
        List<API> supportedAPIs;
        private bool showAvailableAPIs;
        APISettings apiSettings;
        API selectedAPI;

        [MenuItem("Tools/Elrond Unity Tools", false, 60)]
        private static void Init()
        {
            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow));
            window.titleContent = new GUIContent("Elrond Tools - v.0.0.1");
            window.minSize = new Vector2(520, 520);
            window.Show();
        }

        private void OnEnable()
        {
            apiSettings = Resources.Load<APISettings>("APISettingsData");
            if (apiSettings == null)
            {
                CreateAPISettings();
                apiSettings = Resources.Load<APISettings>("APISettingsData");
            }

            supportedAPIs = new List<API>();
            DirectoryInfo dir = new DirectoryInfo($"{Application.dataPath}/GleyPlugins/ElrondUnityTools/Scripts/Provider/APIs");

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
                selectedAPI = supportedAPIs[0];
            }
        }

        private void OnGUI()
        {

            EditorGUILayout.LabelField("API configuration ", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            #region SelectedAPI
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Selected API Name: {selectedAPI.apiName}", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
            #endregion

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height - 100));
            EditorGUILayout.Space();

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
                    if (GUILayout.Button("Select"))
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
            #endregion

            EditorGUILayout.Space();

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
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            CreateSupportedAPIEnum();
            CreateEndPointEnum();
            DeleteOldAPIFIles();
            for (int i = 0; i < supportedAPIs.Count; i++)
            {
                SaveJson(supportedAPIs[i]);
            }
            apiSettings.selectedAPIName = selectedAPI.apiName;
            EditorUtility.SetDirty(apiSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void AddAPI()
        {
            addAPI = false;
            selectedAPI = new API(apiName, baseAddress);
            supportedAPIs.Add(selectedAPI);

        }

        private void DeleteOldAPIFIles()
        {
            DirectoryInfo dir = new DirectoryInfo($"{Application.dataPath}/GleyPlugins/ElrondUnityTools/Scripts/Provider/APIs");

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }

        private void CreateSupportedAPIEnum()
        {
            string text =
            "namespace ElrondUnityTools\n" +
            "{\n" +
            "\tpublic enum SupportedAPIs\n" +
            "\t{\n";
            for (int i = 0; i < supportedAPIs.Count; i++)
            {
                text += "\t\t" + supportedAPIs[i].apiName + ",\n";
                SaveJson(supportedAPIs[i]);
            }
            text += "\t}\n}";
            File.WriteAllText(Application.dataPath + "/GleyPlugins/ElrondUnityTools/Scripts/Provider/SupportedAPIs.cs", text);
        }

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
            //verify
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


            string text =
            "namespace ElrondUnityTools\n" +
            "{\n" +
            "\tpublic enum EndpointNames\n" +
            "\t{\n";
            for (int i = 0; i < enumElements.Count; i++)
            {
                text += "\t\t" + enumElements[i] + ",\n";
            }
            text += "\t}\n}";
            File.WriteAllText(Application.dataPath + "/GleyPlugins/ElrondUnityTools/Scripts/Provider/EndpointNames.cs", text);
        }

        private void SaveJson(API api)
        {
            string json = JsonConvert.SerializeObject(api);
            File.WriteAllText(Application.dataPath + $"/GleyPlugins/ElrondUnityTools/Scripts/Provider/APIs/{api.apiName}.json", json);
        }

        private void CreateAPISettings()
        {
            APISettings asset = CreateInstance<APISettings>();
            string path = "Assets/GleyPlugins/ElrondUnityTools/Resources";
            EditorUtilities.CreateFolder(path);
            AssetDatabase.CreateAsset(asset, $"{path}/APISettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
