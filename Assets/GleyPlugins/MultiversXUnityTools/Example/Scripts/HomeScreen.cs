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
        SupportedAPIs selected;
        APISettings apiSettings;

        private void Start()
        {
            version.text = $"v.{Application.version}";
            apiSettings = Manager.GetApiSettings();
            Debug.Log(apiSettings);
            PopulateDropDownWithEnum(supportedAPIs, selected);
            supportedAPIs.value = (int)(SupportedAPIs)Enum.Parse(typeof(SupportedAPIs), apiSettings.selectedAPIName, true);
            selected = (SupportedAPIs)supportedAPIs.value;
            supportedAPIs.onValueChanged.AddListener(delegate
            {
                DropdownValueChanged(supportedAPIs);
            });
            ActivateWarning();
        }
        //linked to the login options button in editor
        public void LoginOptions()
        {
            DemoScript.Instance.LoadScreen(Screens.Login);
        }

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


        private void DropdownValueChanged(Dropdown dropdown)
        {
            selected = (SupportedAPIs)dropdown.value;
            apiSettings.selectedAPIName = selected.ToString();
            ActivateWarning();
        }

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