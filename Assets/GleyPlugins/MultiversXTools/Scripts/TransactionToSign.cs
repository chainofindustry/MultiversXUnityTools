using Erdcsharp.Domain;
using MultiversXUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiversXUnityTools
{
    public enum TransactionType
    {
        EGLD,
        ESDT,
        NFT,
        SC
    }

    public class TransactionToSign
    {
        public string destination;
        public string value;
        public string data;
        public Token token;
        public TransactionType type;

        public TransactionToSign(string destination, string value, string message)
        {
            this.destination = destination;
            this.value = value;
            data = message;
            type = TransactionType.EGLD;
        }

        public TransactionToSign(string destination, Token token, string value)
        {
            this.destination = destination;
            this.value = value;
            this.token = token;
            type = TransactionType.ESDT;
        }
    }
}