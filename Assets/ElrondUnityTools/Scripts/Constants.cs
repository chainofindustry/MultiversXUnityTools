namespace ElrondUnityTools
{
    public class Constants
    {
        public const string ConnectionManagerObject = "ElrondConnectionManager";
        public const string customBridgeUrl = "https://f.bridge.walletconnect.org";

        //ElrondNetwork setup
        public const Erdcsharp.Configuration.Network networkType = Erdcsharp.Configuration.Network.DevNet;

        //API for POST a signed transaction
        public const string transactionPostAPI = "https://devnet-api.elrond.com/transactions";

        //API to check transaction status
        //{txHash} will be replaced at runtime with the actual transaction hash 
        public const string transactionStatusAPI = "https://devnet-api.elrond.com/transactions/{txHash}?fields=status";


        //API to check for NFTs
        //{address} will be replaced at runtime with the connected address
        public const string getNFTAPI = "https://devnet-api.elrond.com/accounts/{address}/nfts?withScamInfo=true&computeScamInfo=true";
        //public const string getNFTAPI = "https://api.elrond.com/accounts/{address}/nfts?withScamInfo=true&computeScamInfo=true";



        //Maiar display values
        public const string appDescription = "You are using Chain of Industry test login";
        public const string appIcon = "https://vilas.edu.vn/wp-content/uploads/2019/07/SCSS-7-Icon2-150x150.png";
        public const string appName = "Chain of Industry";
        public const string appWebsite = "http://chainofindustry.com/";
    }
}