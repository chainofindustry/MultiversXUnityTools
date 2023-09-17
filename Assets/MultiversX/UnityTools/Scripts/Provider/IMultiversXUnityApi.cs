using Mx.NET.SDK.Provider.API;
using Mx.NET.SDK.Provider.Gateway;
using System.Threading.Tasks;
using ITransactionsProvider = Mx.NET.SDK.Provider.API.ITransactionsProvider;

namespace MultiversX.UnityTools
{
    /// <summary>
    /// Addition to the default MultiversX SDK methods 
    /// </summary>
    public interface IMultiversXUnityApi : IApiProvider, IGatewayProvider, ITransactionsProvider
    { 
        Task<T> GetWalletNfts<T>(string address);

        Task<T> GetWalletTokens<T>(string address);
    }
}