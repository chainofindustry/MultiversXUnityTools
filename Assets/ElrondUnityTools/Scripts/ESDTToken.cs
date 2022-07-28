namespace ElrondUnityTools
{
    public class ESDTToken
    {
        public string name;
        public string identifier;
        public int decimals;

        /// <summary>
        /// ESDT Constructor
        /// </summary>
        /// <param name="name">Name of ESDT toke, used only for display purpose</param>
        /// <param name="identifier">The identifier as used on blockchain(Name+numeric value)</param>
        /// <param name="decimals">number of decimals</param>
        public ESDTToken(string name, string identifier, int decimals)
        {
            this.name = name;
            this.identifier = identifier;
            this.decimals = decimals;
        }
    }
}
