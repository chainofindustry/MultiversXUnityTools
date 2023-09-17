using System.Threading.Tasks;

namespace MultiversX.UnityTools
{
    /// <summary>
    /// Addition to the default MultiversX SDK methods 
    /// </summary>
    public interface IMultiversXUnityApi
    { 
        Task<T> GetWalletNfts<T>(string address);

        Task<T> GetWalletTokens<T>(string address);
    }
}