using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MultiversXUnityTools
{
    public class TransactionProcessed
    {
        public string destination;
        public string value;
        public string data;
        public long gasRequiredForSCExecution;

        public TransactionProcessed(string destination, string value, string data, long gasRequiredForSCExecution)
        {
            this.destination = destination;
            this.value = value;
            this.data = data;
            this.gasRequiredForSCExecution = gasRequiredForSCExecution;
        }

        public TransactionProcessed(TransactionToSign transactionToSign)
        {
            this.destination = transactionToSign.destination;
            this.value = transactionToSign.value;
            this.data = transactionToSign.data;
        }
    }
}
