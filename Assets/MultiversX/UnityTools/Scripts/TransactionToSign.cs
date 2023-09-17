using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;

namespace MultiversX.UnityTools
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
        public string methodName;
        public ESDT token;
        public TransactionType type;
        public string collectionIdentifier;
        public ulong nftNonce;
        public long gas;
        public int quantity;
        public IBinaryType[] args;

        public TransactionToSign(string destination, string value, string message)
        {
            this.destination = destination;
            this.value = value;
            data = message;
            type = TransactionType.EGLD;
        }

        public TransactionToSign(string destination, ESDT token, string value)
        {
            this.destination = destination;
            this.value = value;
            this.token = token;
            type = TransactionType.ESDT;
        }

        public TransactionToSign(string destination, string collectionIdentifier, ulong nftNonce, int quantity)
        {
            this.destination = destination;
            this.collectionIdentifier = collectionIdentifier;
            this.nftNonce = nftNonce;
            this.quantity = quantity;
            type = TransactionType.NFT;
        }

        public TransactionToSign(string scAddress, string methodName, long gas, params IBinaryType[] args)
        {
            this.destination = scAddress;
            this.methodName = methodName;
            this.gas = gas;
            this.args = args;
            type = TransactionType.SC;
        }
    }
}