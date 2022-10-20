using System.Threading.Tasks;
using UnityEngine;

namespace ElrondUnityTools
{
    public class WalletConnectActions : MonoBehaviour
    {
        public async Task<string> SignTransaction(TransactionData transaction)
        {
            var results = await WalletConnect.ActiveSession.ErdSignTransaction(transaction);

            return results;
        }

        public async Task<string> BatchSignTransaction(TransactionData transaction)
        {
            var results = await WalletConnect.ActiveSession.ErdBatchSignTransaction(transaction);

            return results;
        }

    }
}