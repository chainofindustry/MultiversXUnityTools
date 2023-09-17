using Mx.NET.SDK.Configuration;

namespace Mx.NET.SDK.Provider
{
    public interface IGatewayProvider :
        Gateway.IGenericProvider,
        Gateway.IAddressesProvider,
        Gateway.ITransactionsProvider,
        Gateway.INetworkProvider,
        Gateway.INodesProvider,
        Gateway.IBlocksProvider,
        Gateway.IQueryVmProvider,
        Gateway.IESDTProvider
    {
        GatewayNetworkConfiguration NetworkConfiguration { get; }
    }
}
