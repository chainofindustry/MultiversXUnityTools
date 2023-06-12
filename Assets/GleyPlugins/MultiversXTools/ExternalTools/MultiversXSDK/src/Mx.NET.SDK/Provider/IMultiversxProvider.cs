namespace Mx.NET.SDK.Provider
{
    public interface IMultiversxProvider :
        API.IApiProvider,
        API.IAccountsProvider,
        API.IBlocksProvider,
        API.ICollectionsProvider,
        API.INetworkProvider,
        API.INftsProvider,
        API.ITokensProvider,
        API.ITransactionsProvider,
        API.IUsernamesProvider,
        API.IxExchangeProvider,

        Gateway.IGatewayProvider,
        Gateway.INetworkProvider
    { }
}
