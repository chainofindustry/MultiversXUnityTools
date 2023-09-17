using System;

namespace Mx.NET.SDK.Configuration
{
    public class GatewayNetworkConfiguration
    {
        public Network Network { get; }
        public Uri GatewayUri { get; }
        public Uri ExplorerUri { get; set; }
        public Uri WebWalletUri { get; set; }

        /// <summary>
        /// Default MultiversX Gateway network configuration
        /// </summary>
        /// <param name="network">MainNet/DevNet/TestNet</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GatewayNetworkConfiguration(Network network)
        {
            Network = network;

            switch (network)
            {
                case Network.MainNet:
                    GatewayUri = new Uri("https://gateway.multiversx.com/");
                    ExplorerUri = new Uri("https://explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://wallet.multiversx.com/");
                    break;
                case Network.DevNet:
                    GatewayUri = new Uri("https://devnet-gateway.multiversx.com/");
                    ExplorerUri = new Uri("https://devnet-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://devnet-wallet.multiversx.com/");
                    break;
                case Network.DevNet2:
                    GatewayUri = new Uri("https://devnet2-gateway.multiversx.com/");
                    ExplorerUri = new Uri("https://devnet2-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://devnet2-wallet.multiversx.com/");
                    break;
                case Network.TestNet:
                    GatewayUri = new Uri("https://testnet-gateway.multiversx.com/");
                    ExplorerUri = new Uri("https://testnet-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://testnet-wallet.multiversx.com/");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(network), network, null);
            }
        }

        /// <summary>
        /// Custom Gateway network configuration
        /// </summary>
        /// <param name="network">MainNet/DevNet/TestNet</param>
        /// <param name="gatewayUri">Gateway custom URI</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GatewayNetworkConfiguration(Network network, Uri gatewayUri)
        {
            Network = network;
            GatewayUri = gatewayUri;

            switch (network)
            {
                case Network.MainNet:
                    ExplorerUri = new Uri("https://explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://wallet.multiversx.com/");
                    break;
                case Network.DevNet:
                    ExplorerUri = new Uri("https://devnet-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://devnet-wallet.multiversx.com/");
                    break;
                case Network.DevNet2:
                    ExplorerUri = new Uri("https://devnet2-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://devnet2-wallet.multiversx.com/");
                    break;
                case Network.TestNet:
                    ExplorerUri = new Uri("https://testnet-explorer.multiversx.com/");
                    WebWalletUri = new Uri("https://testnet-wallet.multiversx.com/");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(network), network, null);
            }
        }
    }
}
