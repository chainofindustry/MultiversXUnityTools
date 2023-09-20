using Mx.NET.SDK.Core.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace MultiversX.UnityTools.Examples
{
    public class SignMessageScreen : GenericUIScreen
    {
        public InputField message;
        public Text result;

        public override void Init(params object[] args)
        {
            message.text = "Do you want to sign this message?";
            base.Init(args);
        }

        public void Back()
        {
            DemoScript.Instance.LoadScreen(Screens.Connected);
        }

        public void SignMessage()
        {
            API.SignMessage(message.text, CompleteMethod);
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
