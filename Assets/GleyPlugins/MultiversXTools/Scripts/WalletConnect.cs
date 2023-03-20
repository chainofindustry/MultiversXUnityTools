using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using WalletConnectSharp.Common.Model.Errors;
using WalletConnectSharp.Core.Models.Pairing;
using WalletConnectSharp.Events;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Storage;

namespace MultiversXUnityTools
{
    public class WalletConnect : MonoBehaviour
    {
        public static class GenerateAuthToken
        {
            public static string Random()
            {
                const string src = "abcdefghijklmnopqrstuvwxyz0123456789";
                int length = 32;
                StringBuilder sb = new();
                System.Random rand = new();
                for (var i = 0; i < length; i++)
                {
                    var c = src[rand.Next(0, src.Length)];
                    sb.Append(c);
                }

                return sb.ToString();
            }
        }

        private const string PROJECT_ID = "39f3dc0a2c604ec9885799f9fc5feb7c";
        private const string SAVE_PATH = "/wc/sessionData.json";
        private const string MVX_NAMESPACE = "multiversx";

        private const string ACCOUNTS_CHANGED = "accountsChanged";

        private WalletConnectSignClient client;
        private SessionStruct sessionStruct;
        private ConnectedData connectedData;
        private UnityAction OnWalletDisconnected;
        private string authToken;
        private string account;
        private bool connected;
        private bool disconnected;

        public async Task<string> Connect(Metadata appMetadata, string chainId, UnityAction<string> OnSessionConnected, UnityAction OnWalletDisconnected)
        {
            this.OnWalletDisconnected = OnWalletDisconnected;
            var appData = new SignClientOptions()
            {
                ProjectId = PROJECT_ID,
                Metadata = appMetadata,
                Storage = new FileSystemStorage(Application.persistentDataPath + SAVE_PATH)
            };

            var xPortalData = new ConnectOptions()
            {
                RequiredNamespaces = new RequiredNamespaces()
                {
                    {
                        MVX_NAMESPACE, new RequiredNamespace()
                        {
                            Methods = new[]
                            {
                                MultiversXRpcMethods.SIGN_TRANSACTION,
                                MultiversXRpcMethods.SIGN_TRANSACTIONS,
                                MultiversXRpcMethods.SIGN_MESSAGE,
                                MultiversXRpcMethods.SIGN_LOGIN_TOKEN,
                                MultiversXRpcMethods.CANCEL_ACTION
                            },
                            Chains = new[]
                            {
                                $"{MVX_NAMESPACE}:{chainId}"
                            },
                            Events = new[]
                            {
                               ACCOUNTS_CHANGED
                            }
                        }
                    }
                }
            };

            if (client == null)
            {
                try
                {
                    client = await WalletConnectSignClient.Init(appData);
                }
                catch (Exception e)
                {
                    throw (e);
                }
                client.On(EngineEvents.SessionDelete, WalletDisconnected);
            }

            var sessions = client.Find(xPortalData.RequiredNamespaces);

            if (sessions.Length > 0)
            {
                sessionStruct = sessions[0];
            }
            else
            {
                authToken = GenerateAuthToken.Random();
                try
                {
                    connectedData = await client.Connect(xPortalData);
                    OnSessionConnected?.Invoke(connectedData.Uri);
                    sessionStruct = await connectedData.Approval;
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }
            try
            {
                account = sessionStruct.Namespaces[MVX_NAMESPACE].Accounts[0].Split(":")[2];
            }
            catch (Exception e)
            {
                throw (e);
            }
            connected = true;
            return account;
        }


        public async Task Disconnect()
        {
            await client.Disconnect(sessionStruct.Topic, ErrorResponse.FromErrorType(ErrorType.USER_DISCONNECTED));
            WalletDisconnected();
        }


        public async Task<string> SignTransaction(TransactionData transaction)
        {
            OpenMobileWallet();
            SignTransactionResponse signature = await client.Request<SignTransaction, SignTransactionResponse>(sessionStruct.Topic, new SignTransaction(transaction));
            return signature.Signature;
        }

        public async Task<string[]> SignTransactions(TransactionData[] transactions)
        {
            OpenMobileWallet();
            string[] result = new string[transactions.Length];
            SignTransactionsResponse signatures = await client.Request<SignTransactions, SignTransactionsResponse>(sessionStruct.Topic, new SignTransactions(transactions));
            Debug.Log(signatures);
            Debug.Log(signatures.Signatures);
            for(int i=0;i<signatures.Signatures.Length;i++)
            {
                result[i] = signatures.Signatures[i].Signature;
            }
            return result;
        }

        public async Task<string> SignMessage(string message)
        {
            SignMessageResponse signature = await client.Request<SignMessage, SignMessageResponse>(sessionStruct.Topic, new SignMessage(message));
            return signature.Signature;
        }

        void OpenMobileWallet()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            string maiarUrl = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com" + "/wc";
            Debug.Log($"{maiarUrl}");
            Application.OpenURL(maiarUrl);
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }


        public void OpenDeepLink()
        {
            if (connectedData == null)
            {
                Debug.LogError("Wallet Connect not ready, wait for the connection");

                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            string maiarUrl = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + UnityWebRequest.EscapeURL($"{connectedData.Uri}&token={authToken}");
            Debug.Log("[WalletConnect] Opening URL: " + maiarUrl);
            Application.OpenURL(maiarUrl);
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }


        public bool IsConnected()
        {
            return connected;
        }


        void WalletDisconnected()
        {
            disconnected = true;
            connected = !disconnected;
            connectedData = null;

        }


        private void Update()
        {
            if (disconnected == true)
            {
                OnWalletDisconnected?.Invoke();
                disconnected = false;
            }
        }
    }
}