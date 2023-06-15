using MultiversXUnityTools;
using Mx.NET.SDK.Core.Domain;
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

        private void CompleteMethod(CompleteCallback<SignableMessage> result)
        {
            if (result.status == OperationStatus.Success)
            {
                this.result.text = $"Message: {result.data.Message}\n" +
                    $"Address: {result.data.Address}\n" +
                    $"Signature: {result.data.Signature}";
            }
            else
            {
                this.result.text = $"{result.status} {result.errorMessage}";
            }
        }
    }
}
