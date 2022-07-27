namespace ElrondUnityTools
{
    public class ESDTToken
    {
        public string name;
        public string identifier;
        public int decimals;

        public ESDTToken(string name, string identifier, int decimals)
        {
            this.name = name;
            this.identifier = identifier;
            this.decimals = decimals;
        }
    }
}
