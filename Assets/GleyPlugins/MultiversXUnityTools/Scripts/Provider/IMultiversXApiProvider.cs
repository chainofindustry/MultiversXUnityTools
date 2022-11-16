using Erdcsharp.Provider;
using System.Threading.Tasks;

namespace MultiversXUnityTools
{
    /// <summary>
    /// Addition to the default MultiversX SDK methods 
    /// </summary>
    public interface IMultiversXApiProvider : IElrondProvider
    {
        Task<TransactionResponse> GetTransactionDetails(string txHash);

        Task<T> GetRequest<T>(string url);

        Task<T> PostRequest<T>(string url, string jsonData);

        Task<T> GetWalletNfts<T>(string address);

        Task<T> GetWalletTokens<T>(string address);
    }
}