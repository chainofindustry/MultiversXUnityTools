using MultiversXUnityTools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversXUnityExamples
{
    public class HomeScreen : GenericUIScreen
    {
        public Text version;
        public Dropdown supportedAPIs;
        public GameObject warning;

        private APISettings apiSettings;
        private SupportedAPIs selected;


        private void Start()
        {
            version.text = $"v.{Application.version}";
            apiSettings = Manager.GetApiSettings();

            //load all available API and populate selection dropdown
            PopulateDropDownWithEnum(supportedAPIs, selected);
            supportedAPIs.value = (int)(SupportedAPIs)Enum.Parse(typeof(SupportedAPIs), apiSettings.selectedAPIName, true);
            selected = (SupportedAPIs)supportedAPIs.value;
            supportedAPIs.onValueChanged.AddListener(delegate
            {
                DropdownValueChanged(supportedAPIs);
            });
            //if selected API is Mainnet show a warning
            ActivateWarning();
        }


        //linked to the login options button in editor
        public void LoginOptions()
        {
            DemoScript.Instance.LoadScreen(Screens.Login);
        }


        /// <summary>
        /// Populate dropdown with available API enum options
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="targetEnum"></param>
        void PopulateDropDownWithEnum(Dropdown dropdown, Enum targetEnum)
        {
            Type enumType = targetEnum.GetType();
            List<Dropdown.OptionData> newOptions = new List<Dropdown.OptionData>();

            for (int i = 0; i < Enum.GetNames(enumType).Length; i++)
            {
                newOptions.Add(new Dropdown.OptionData(Enum.GetName(enumType, i)));
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
        }


        /// <summary>
        /// Callback triggered when dropdown value modifies
        /// </summary>
        /// <param name="dropdown"></param>
        private void DropdownValueChanged(Dropdown dropdown)
        {
            selected = (SupportedAPIs)dropdown.value;
            //select the active API according to user selection
            apiSettings.selectedAPIName = selected.ToString();
            ActivateWarning();
        }


        //display a text if selected API is for mainnet
        void ActivateWarning()
        {
            if (selected == SupportedAPIs.MultiversXApiMainnet)
            {
                warning.SetActive(true);
            }
            else
            {
                warning.SetActive(false);
            }
        }
    }
}