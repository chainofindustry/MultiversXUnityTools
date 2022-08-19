namespace ElrondUnityTools
{
    public class QueryResponse
    {
        public Data data { get; set; }
        public string error { get; set; }
        public string code { get; set; }
    }

    public class Data
    {
        public SCData data { get; set; }
    }

    [System.Serializable]
    public class SCData
    {
        public string[] returnData { get; set; }
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public int gasRemaining { get; set; }
        public int gasRefund { get; set; }
        public Outputaccounts outputAccounts { get; set; }
        public object[] deletedAccounts { get; set; }
        public object[] touchedAccounts { get; set; }
        public object[] logs { get; set; }
    }

    [System.Serializable]
    public class Outputaccounts
    {
        public account _00000000000000000500de814a98701cc9374ae49d23f0e45ba98fe165777e40 { get; set; }
    }

    [System.Serializable]
    public class account
    {
        public string address { get; set; }
        public int nonce { get; set; }
        public object balance { get; set; }
        public int balanceDelta { get; set; }
        public Storageupdates storageUpdates { get; set; }
        public object code { get; set; }
        public object codeMetaData { get; set; }
        public object[] outputTransfers { get; set; }
        public int callType { get; set; }
    }

    public class Storageupdates
    {
    }

}