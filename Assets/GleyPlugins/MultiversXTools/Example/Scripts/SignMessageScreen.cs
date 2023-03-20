using MultiversXUnityTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversXUnityExamples
{
    public class SignMessageScreen : GenericUIScreen
    {
        public InputField message;
        public Text result;

        public override void Init(params object[] args)
        {
            Debug.Log("Init");
            message.text = "Do you want to sign this message?";
            base.Init(args);
        }

        public void Back()
        {
            DemoScript.Instance.LoadScreen(Screens.Connected);
        }

        public void SignMessage()
        {
            Manager.SignMessage(message.text, CompleteMethod);
        }

        private void CompleteMethod(OperationStatus status, string message)
        {
            if(status== OperationStatus.Complete)
            {
                result.text = message;
            }
            else
            {
                result.text = $"{status} {message}";
            }
        }
    }
}
