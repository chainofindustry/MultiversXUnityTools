using System;
using System.Text;

namespace MultiversXUnityTools
{
    public class TransactionData
    {
        public long nonce;
        public string from;
        public string to;
        public string amount;
        public string gasPrice;
        public string gasLimit;
        public string data;
        public string chainId;
        public int version;
    }

    public class SignedTransactionData
    {
        public long nonce;
        public string sender;
        public string receiver;
        public string value;
        public long gasPrice;
        public long gasLimit;
        public string data;
        public string chainId;
        public int version;
        public string signature;

        public SignedTransactionData(TransactionData tx, string signature)
        {
            nonce = tx.nonce;
            sender = tx.from;
            receiver = tx.to;
            value = tx.amount;
            gasPrice = long.Parse(tx.gasPrice);
            gasLimit = long.Parse(tx.gasLimit);
            data = Convert.ToBase64String(Encoding.UTF8.GetBytes(tx.data));
            chainId = tx.chainId;
            version = tx.version;
            this.signature = signature;
        }
    }
}
