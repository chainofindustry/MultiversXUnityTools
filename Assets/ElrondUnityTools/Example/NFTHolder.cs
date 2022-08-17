using ElrondUnityTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class NFTHolder : MonoBehaviour
    {
        public Image image;
        public Text name;
        public string tokenIdentifier;
        public int nonce;
        public void Initialize(string name, string tokenIdentifier, int nonce)
        {
            this.name.text = name;
            this.tokenIdentifier = tokenIdentifier;
            this.nonce = nonce;
        }

        public void SetImage(Sprite image)
        {
            this.image.sprite = image;
        }

        public void SendNFT()
        {
            Debug.Log("SEND NFT");

            ElrondUnityTools.Manager.SendNFT(FindObjectOfType<DemoScript>().nftDestination.text, tokenIdentifier, nonce, 1, Complete);
        }

        private void Complete(OperationStatus arg0, string arg1)
        {
            Debug.Log(arg0 + " " + arg1);
        }
    }
}