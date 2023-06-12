using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.WalletConnect;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace MultiversXUnityTools
{
    public class WalletConnectUnity : MonoBehaviour
    {
        private WalletConnect walletConnect;
        private UnityAction OnWalletDisconnected;
        private bool disconnected;
        private bool sessionConnected;

        internal async Task<WalletConnectUnity> Initialize(WalletConnectSharp.Core.Models.Pairing.Metadata appMetadata, string projectID, string chainId, string filePath)
        {
            walletConnect = new WalletConnect(appMetadata, projectID, chainId, filePath);
            walletConnect.OnSessionDeleteEvent += WalletDisconnected;
            await walletConnect.ClientInit();
            return this;
        }

        internal async Task<string> Connect(UnityAction<string> OnSessionConnected, UnityAction OnWalletDisconnected)
        {
            this.OnWalletDisconnected = OnWalletDisconnected;

            if (!walletConnect.TryReconnect())
            {
                try
                {
                    await walletConnect.Initialize();
                    sessionConnected = true;
                    OnSessionConnected?.Invoke(walletConnect.URI);
                    await walletConnect.Connect();
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }
            return walletConnect.Address;
        }


        public bool IsConnected()
        {
            return sessionConnected;
            //return walletConnect.IsConnected();
        }


        public async Task<TransactionRequestDto> Sign(TransactionRequest transaction)
        {
            OpenMobileWallet();
            return await walletConnect.Sign(transaction);
        }


        public async Task<TransactionRequestDto[]> MultiSign(TransactionRequest[] transactionsRequest)
        {
            OpenMobileWallet();
            return await walletConnect.MultiSign(transactionsRequest);
        }


        public async Task<SignableMessage> SignMessage(string message)
        {
            OpenMobileWallet();
            return await walletConnect.SignMessage(message);
        }


        private void WalletDisconnected(object sender, EventArgs e)
        {
            WalletDisconnected();
        }


        public async Task Disconnect()
        {
            WalletDisconnected();
            await walletConnect.Disconnect();
        }


        public void OpenDeepLink()
        {
            if (!sessionConnected)
            {
                Debug.LogError("Wallet Connect not ready, wait for the connection");
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            string xPortal = $"{WalletConnectGeneric.MAIAR_BRIDGE_URL}?wallet-connect={UnityWebRequest.EscapeURL(walletConnect.URI)}";
            Debug.Log("[WalletConnect] Opening URL: " + xPortal);
            Application.OpenURL(xPortal);
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }


        private void OpenMobileWallet()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            string xPortalURL = $"{WalletConnectGeneric.MAIAR_BRIDGE_URL}/wc";
            Debug.Log($"{xPortalURL}");
            Application.OpenURL(xPortalURL);
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }

        private void WalletDisconnected()
        {
            disconnected = true;
            sessionConnected = false;
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