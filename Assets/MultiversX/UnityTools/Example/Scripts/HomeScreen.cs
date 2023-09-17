using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network = Mx.NET.SDK.Configuration.Network;

namespace MultiversX.UnityTools.Examples
{
    public class HomeScreen : GenericUIScreen
    {
        public Text version;
        public Dropdown supportedAPIs;
        public GameObject warning;

        private AppSettings apiSettings;
        private Network selected;


        private void Start()
        {
            version.text = $"v.{Application.version}";
            apiSettings = API.GetApiSettings();

            //load all available API and populate selection dropdown
            selected = apiSettings.selectedNetwork;
            PopulateDropDownWithEnum(supportedAPIs, selected);
            supportedAPIs.value = (int)selected;

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
            selected = (Network)dropdown.value;
            //select the active API according to user selection
            apiSettings.selectedNetwork = selected;
            ActivateWarning();
        }


        //display a text if selected API is for mainnet
        void ActivateWarning()
        {
            if (selected == Network.MainNet)
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