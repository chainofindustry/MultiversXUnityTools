namespace MultiversXUnityTools
{
    public class TransactionResponse
    {
        public string txHash { get; set; }
        public int gasLimit { get; set; }
        public int gasPrice { get; set; }
        public int gasUsed { get; set; }
        public string miniBlockHash { get; set; }
        public int nonce { get; set; }
        public string receiver { get; set; }
        public int receiverShard { get; set; }
        public int round { get; set; }
        public string sender { get; set; }
        public int senderShard { get; set; }
        public string signature { get; set; }
        public string status { get; set; }
        public string value { get; set; }
        public string fee { get; set; }
        public int timestamp { get; set; }
        public string data { get; set; }
        public string function { get; set; }
        public Action action { get; set; }
        public float price { get; set; }
        public Logs logs { get; set; }
        public Operation[] operations { get; set; }
    }

    public class Action
    {
        public string category { get; set; }
        public string name { get; set; }
    }

    public class Logs
    {
        public string id { get; set; }
        public string address { get; set; }
        public Event[] events { get; set; }
    }

    public class Event
    {
        public string identifier { get; set; }
        public string address { get; set; }
        public string data { get; set; }
        public string[] topics { get; set; }
        public int order { get; set; }
    }

    public class Operation
    {
        public string id { get; set; }
        public string action { get; set; }
        public string type { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public string data { get; set; }
        public string message { get; set; }
    }
}