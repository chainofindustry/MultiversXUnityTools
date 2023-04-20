using System;

namespace Mx.NET.SDK.Configuration
{
    public class MultiversxNetworkConfiguration
    {
        public Network Network { get; }
        public Uri APIUri { get; }
        public Uri GatewayUri { get; }
        public Uri ExplorerUri { get; }
        public Uri WebWalletUri { get; set; }

        public MultiversxNetworkConfiguration(Network network)
        {
            Network = network;
            switch (network)
            {
                case Network.MainNet:
                    APIUri = new Uri("https://api.multiversx.com/");
                    GatewayUri = new Uri("https://gateway.multiversx.com/");
                    ExplorerUri = new Uri("https://explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://wallet.multiversx.com/");
                    break;
                case Network.DevNet:
                    APIUri = new Uri("https://devnet-api.multiversx.com/");
                    GatewayUri = new Uri("https://devnet-gateway.multiversx.com/");
                    ExplorerUri = new Uri("https://devnet-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://devnet-wallet.multiversx.com/");
                    break;
                case Network.TestNet:
                    APIUri = new Uri("https://testnet-api.multiversx.com/");
                    GatewayUri = new Uri("https://testnet-gateway.multiversx.com/");
                    ExplorerUri = new Uri("https://testnet-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://testnet-wallet.multiversx.com/");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(network), network, null);
            }
        }
    }
}
