namespace MultiversXUnityTools
{

    [System.Serializable]
    public class SCQuery
    {
        public string scAddress;
        public string funcName;
        public string[] args;

        public SCQuery(string scAddress, string funcName, string[] args)
        {
            this.scAddress = scAddress;
            this.funcName = funcName;
            if (args.Length > 0)
            {
                if (!string.IsNullOrEmpty(args[0]))
                {
                    this.args = args;
                }
            }
        }
    }
}