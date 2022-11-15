using Erdcsharp.Provider;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiversXUnityTools
{
    public interface IElrondApiProvider : IElrondProvider
    {
        Task<T> GetRequest<T>(string url);

        Task<T> PostRequest<T>(string url, string jsonData);

        Task<T> GetWalletNfts<T>(string address);

        Task<T> GetWalletTokens<T>(string address);
    }
}