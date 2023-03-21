using WalletConnectSharp.Network.Models;

namespace MultiversXUnityTools
{
    [RpcMethod(MultiversXRpcMethods.SIGN_MESSAGE)]
    public class SignMessage 
    {
        public long messageSize;
        public string message; 

        public SignMessage(string message) 
        {
            this.message = message;
            messageSize = System.Text.Encoding.ASCII.GetBytes(message).Length;
        }
    }

    public class SignMessageResponse
    {
        public string Signature { get; set; }
    }
}
