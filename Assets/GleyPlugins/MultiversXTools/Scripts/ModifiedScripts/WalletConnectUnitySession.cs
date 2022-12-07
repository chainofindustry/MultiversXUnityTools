using System.Threading.Tasks;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Events;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Network;

namespace MultiversXUnityTools
{
    public class WalletConnectUnitySession : WalletConnectSession
    {
        private WalletConnect unityObjectSource;

        public WalletConnectUnitySession(SavedSession savedSession, WalletConnect source, ITransport transport = null, ICipher cipher = null, EventDelegator eventDelegator = null) : base(savedSession, transport, cipher, eventDelegator)
        {
            this.unityObjectSource = source;
        }

        public WalletConnectUnitySession(ClientMeta clientMeta, WalletConnect source, string bridgeUrl = null, ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null) : base(clientMeta, bridgeUrl, transport, cipher, chainId, eventDelegator)
        {
            this.unityObjectSource = source;
        }

        internal string KeyData
        {
            get
            {
                return base._key;
            }
        }

        internal async Task<WCSessionData> SourceConnectSession()
        {
            Connecting = true;
            var result = await base.ConnectSession();
            Connecting = false;
            return result;
        }

        public override async Task Connect()
        {
            await ConnectSession();
        }

        public override async Task<WCSessionData> ConnectSession()
        {
            return await unityObjectSource.Connect();
        }

        public async Task<string> ErdBatchSignTransaction(params TransactionData[] transaction)
        {
            EnsureNotDisconnected();

            var request = new ErdBatchSignTransaction(transaction);

            var response = await Send<ErdBatchSignTransaction, ErdResponse>(request);

            return response.Result;
        }

        public async Task<string> ErdSignTransaction(TransactionData transaction)
        {
            EnsureNotDisconnected();

            var request = new ErdSignTransaction(transaction);

            var response = await Send<ErdSignTransaction, ErdResponse>(request);

            return response.Result;
        }
    }
}