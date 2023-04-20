namespace MultiversXUnityTools
{
    /// <summary>
    /// MultiversX transaction data structure
    /// </summary>
    public class TransactionData
    {
        public ulong nonce;
        public string sender;
        public string receiver;
        public string value;
        public long gasPrice;
        public long gasLimit;
        public string data;
        public string chainID;
        public int version;
    }
}
