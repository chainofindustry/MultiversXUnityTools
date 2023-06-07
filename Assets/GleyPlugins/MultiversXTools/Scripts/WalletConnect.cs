using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.WalletConnectV2.Data;
using Mx.NET.SDK.WalletConnectV2.Helper;
using Mx.NET.SDK.WalletConnectV2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
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


        private WalletConnectSignClient client;
        private SessionStruct sessionStruct;
        private ConnectedData connectedData;
        private UnityAction OnWalletDisconnected;
        private string authToken;
        private string account;
        private bool connected;
        private bool disconnected;

        public async Task<string> Connect(WalletConnectSharp.Core.Models.Pairing.Metadata appMetadata, string chainId, UnityAction<string> OnSessionConnected, UnityAction OnWalletDisconnected)
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
                         MultiversXRpcMethods.MVX_NAMESPACE, new RequiredNamespace()
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
                                $"{ MultiversXRpcMethods.MVX_NAMESPACE}:{chainId}"
                            },
                            Events = new[]
                            {
                               MultiversXRpcMethods.ACCOUNTS_CHANGED
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
                account = sessionStruct.Namespaces[MultiversXRpcMethods.MVX_NAMESPACE].Accounts[0].Split(":")[2];
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
            WalletDisconnected();
            await client.Disconnect(sessionStruct.Topic, ErrorResponse.FromErrorType(ErrorType.USER_DISCONNECTED));
        }


        public async Task<TransactionRequestDto> Sign(RequestData transaction)
        {
            OpenMobileWallet();
            SignTransactionRequest request = new SignTransactionRequest() { transaction = transaction };
            SignTransactionResponse response = await client.Request<SignTransactionRequest, SignTransactionResponse>(sessionStruct.Topic, request);
            return transaction.ToSignedTransaction(response.Signature);
        }

        public async Task<TransactionRequestDto[]> MultiSign(TransactionRequest[] transactionsRequest)
        {
            OpenMobileWallet();

            var request = transactionsRequest.GetSignTransactionsRequest();
            var response = await client.Request<SignTransactionsRequest, SignTransactionsResponse>(sessionStruct.Topic, request);

            var transactions = new List<TransactionRequestDto>();
            for (var i = 0; i < response.Signatures.Length; i++)
            {
                var transactionRequestDto = transactionsRequest[i].GetTransactionRequest();
                transactionRequestDto.Signature = response.Signatures[i].Signature;
                transactions.Add(transactionRequestDto);
            }

            return transactions.ToArray();
        }

        public async Task<string> SignMessage(string message)
        {
            OpenMobileWallet();
            SignMessageResponse signature = await client.Request<SignMessage, SignMessageResponse>(sessionStruct.Topic, new SignMessage(message));
            return signature.Signature;
        }

        void OpenMobileWallet()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            string xPortalURL = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://xportal.com" + "/wc";
            Debug.Log($"{xPortalURL}");
            Application.OpenURL(xPortalURL);
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
            string xPortal = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + UnityWebRequest.EscapeURL($"{connectedData.Uri}&token={authToken}");
            Debug.Log("[WalletConnect] Opening URL: " + xPortal);
            Application.OpenURL(xPortal);
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