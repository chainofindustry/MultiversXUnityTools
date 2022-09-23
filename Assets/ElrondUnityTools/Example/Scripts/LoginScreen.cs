using Erdcsharp.Domain;
using Erdcsharp.Provider.Dtos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class LoginScreen : GenericUIScreen
    {
        public Image qrImage;

        public override void Init(params object[] args)
        {
            DemoScript.Instance.Connect(qrImage);
            
        }

        //linked to the login button in editor
        public void Login()
        {
            ElrondUnityTools.Manager.DeepLinkLogin();
        }
    }
}