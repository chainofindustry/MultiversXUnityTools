using System;
using System.Text;

namespace MultiversXUnityTools
{
    /// <summary>
    /// MultiversX transaction data structure
    /// </summary>
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
}
