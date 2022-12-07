using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiversXUnityTools
{

    public class MultiversXTransaction
    {
        public string Status { get; private set; }
        public string TxHash { get; }

        private Operation[] operations;

        private Logs logs;

        public MultiversXTransaction(string hash)
        {
            TxHash = hash;
        }

        /// <summary>
        /// Returns whether the transaction is pending (e.g. in mempool).
        /// </summary>
        /// <returns></returns>
        public bool IsPending()
        {
            return Status == "received" || Status == "pending" || Status == "partially-executed";
        }

        /// <summary>
        /// Returns whether the transaction has been executed (not necessarily with success)
        /// </summary>
        /// <returns></returns>
        public bool IsExecuted()
        {
            return IsSuccessful() || IsInvalid() || IsFailed();
        }

        /// <summary>
        /// Returns whether the transaction has been executed successfully.
        /// </summary>
        /// <returns></returns>
        public bool IsSuccessful()
        {
            return Status == "executed" || Status == "success" || Status == "successful";
        }

        /// <summary>
        /// Returns whether the transaction has been executed, but with a failure.
        /// </summary>
        /// <returns></returns>
        public bool IsFailed()
        {
            return Status == "fail" || Status == "failed" || Status == "unsuccessful" || IsInvalid();
        }

        /// <summary>
        /// Returns whether the transaction has been executed, but marked as invalid (e.g. due to "insufficient funds")
        /// </summary>
        /// <returns></returns>
        public bool IsInvalid()
        {
            return Status == "invalid";
        }

        public async Task Sync(IMultiversXApiProvider provider)
        {
            TransactionResponse transaction = await provider.GetTransactionDetails(TxHash);
            if (transaction.operations != null)
            {
                operations = transaction.operations;
            }

            if (transaction.logs != null)
            {
                logs = transaction.logs;
            }

            Status = transaction.status;
        }

        public bool EnsureTransactionSuccess(out string message)
        {
            if (!IsExecuted())
            {
                message = $"Cannot reach Executed status for tx : '{TxHash}'";
                return false;
            }
            if (operations != null && operations.Any(s => !string.IsNullOrEmpty(s.message)))
            {
                var returnMessages = operations.Select(x => x.message).ToArray();
                var aggregateMessage = string.Join(Environment.NewLine, returnMessages);
                message = $"Transaction tx : '{TxHash}' has some error : {aggregateMessage}";
                return false;
            }

            if (IsFailed())
            {
                message = $"Transaction failed for tx : '{TxHash}' Logs: {ReadLogs(logs)}";
                return false;
            }

            if (IsInvalid())
            {
                message = $"Transaction is invalid for tx : '{TxHash}' Logs: {ReadLogs(logs)}";
                return false;
            }

            if (!IsSuccessful())
            {
                message = $"Transaction is invalid for tx : '{TxHash}'";
                return false;
            }
            message = "Success";
            return true;
        }

        /// <summary>
        /// Wait for the execution of the transaction
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task AwaitExecuted(IMultiversXApiProvider provider, TimeSpan? timeout = null)
        {
            if (!timeout.HasValue)
                timeout = TimeSpan.FromSeconds(60);

            var currentIteration = 0;
            do
            {
                await Task.Delay(1000); // 1 second
                await Sync(provider);
                currentIteration++;
            } while (!IsExecuted() && currentIteration < timeout.Value.TotalSeconds); 
        }

        private string ReadLogs(Logs logs)
        {
            string log = "";
            if (logs != null)
            {
                if (logs.events != null)
                {
                    for (int i = 0; i < logs.events.Length; i++)
                    {
                        log += logs.events[i].identifier;
                        if (logs.events[i].topics != null)
                        {
                            for (int j = 1; j < logs.events[i].topics.Length; j++)
                            {
                                log += $" {Encoding.UTF8.GetString(Convert.FromBase64String(logs.events[i].topics[j]))} ";
                            }
                        }
                        log += "; ";
                    }
                }
            }
            return log;
        }
    }
}