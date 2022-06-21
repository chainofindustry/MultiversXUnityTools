using Erdcsharp.Provider.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ElrondUnityTools
{
    public class Manager
    {
        public static void Connect(UnityAction<AccountDto> OnWalletConnected, UnityAction OnWalletDisconnected, Image qrImage)
        {
            ConnectionManager.Instance.Connect(OnWalletConnected, OnWalletDisconnected,qrImage);
        }

        public static bool IsWalletConnected()
        {
            return ConnectionManager.Instance.IsWalletConnected();
        }

        public static void DeepLinkLogin()
        {
            ConnectionManager.Instance.DeepLinkLogin();
        }


        public static void Disconnect()
        {
            ConnectionManager.Instance.Disconnect();
        }

        public static void SendTransaction()
        {

        }
    }
}