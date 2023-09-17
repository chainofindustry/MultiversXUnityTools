using System;
using System.Linq;
using System.Threading.Tasks;
using Mx.NET.SDK.Core.Domain.Codec;
using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain.Helper;
using Mx.NET.SDK.Provider;

namespace Mx.NET.SDK.Domain.Data.Transactions
{
    public class Transaction
    {
        /// <summary>
        /// Transaction hash
        /// </summary>
        public string TxHash { get; private set; }

        /// <summary>
        /// Transaction gas limit
        /// </summary>
        public GasLimit GasLimit { get; private set; }

        /// <summary>
        /// Network gas price
        /// </summary>
        public long GasPrice { get; private set; }

        /// <summary>
        /// Transaction gas used
        /// </summary>
        public GasLimit GasUsed { get; private set; }

        /// <summary>
        /// Transaction miniblock hash
        /// </summary>
        public string MiniBlockHash { get; private set; }

        /// <summary>
        /// Sender nonce 
        /// </summary>
        public long Nonce { get; private set; }

        /// <summary>
        /// Receiver address
        /// </summary>
        public Address Receiver { get; private set; }

        /// <summary>
        /// Receiver assets
        /// </summary>
        public Assets ReceiverAssets { get; private set; }

        /// <summary>
        /// Receiver shard
        /// </summary>
        public long ReceiverShard { get; private set; }

        /// <summary>
        /// Transaction round
        /// </summary>
        public long Round { get; private set; }

        /// <summary>
        /// Sender address
        /// </summary>
        public Address Sender { get; private set; }

        /// <summary>
        /// Sender assets
        /// </summary>
        public Assets SenderAssets { get; private set; }

        /// <summary>
        /// Sender shard
        /// </summary>
        public long SenderShard { get; private set; }

        /// <summary>
        /// Transaction signature
        /// </summary>
        public string Signature { get; private set; }

        /// <summary>
        /// Transaction status
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Transaction EGLD price at that moment
        /// </summary>
        public string EGLDPrice { get; private set; }

        /// <summary>
        /// Transaction EGLD transferred
        /// </summary>
        public ESDTAmount Value { get; private set; }

        /// <summary>
        /// Transaction fee
        /// </summary>
        public ESDTAmount Fee { get; private set; }

        /// <summary>
        /// Transaction creation date
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Transaction data (decoded)
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Transaction function called
        /// </summary>
        public string Function { get; private set; }

        /// <summary>
        /// Transction action
        /// </summary>
        public Common.Action Action { get; private set; }

        /// <summary>
        /// Transction smart contract results
        /// </summary>
        public SmartContractResult[] SmartContractResult { get; private set; }

        /// <summary>
        /// Transaction logs
        /// </summary>
        public Log Logs { get; private set; }

        /// <summary>
        /// Transaction operations executed
        /// </summary>
        public Operation[] Operations { get; private set; }

        private Transaction() { }

        public Transaction(string hash)
        {
            TxHash = hash;
        }

        public static Transaction From(string txHash)
        {
            return new Transaction(txHash);
        }

        public static Transaction[] From(string[] txHashes)
        {
            return txHashes.Select(txHash => new Transaction(txHash)).ToArray();
        }

        /// <summary>
        /// Creates a new Transaction from data
        /// </summary>
        /// <param name="transaction">Transaction Data Object from API</param>
        /// <returns>Transaction object</returns>
        public static Transaction From(TransactionDto transaction)
        {
            return new Transaction()
            {
                TxHash = transaction.TxHash,
                GasLimit = new GasLimit(transaction.GasLimit),
                GasPrice = transaction.GasPrice,
                GasUsed = new GasLimit(transaction.GasUsed),
                MiniBlockHash = transaction.MiniBlockHash,
                Nonce = transaction.Nonce,
                Receiver = Address.FromBech32(transaction.Receiver),
                ReceiverAssets = Assets.From(transaction.ReceiverAssets),
                ReceiverShard = transaction.ReceiverShard,
                Round = transaction.Round,
                Sender = Address.FromBech32(transaction.Sender),
                SenderAssets = Assets.From(transaction.SenderAssets),
                SenderShard = transaction.SenderShard,
                Signature = transaction.Signature,
                Status = transaction.Status,
                Value = ESDTAmount.From(transaction.Value),
                Fee = ESDTAmount.From(transaction.Fee),
                CreationDate = transaction.Timestamp.ToDateTime(),
                Data = DataCoder.DecodeData(transaction.Data),
                Function = transaction.Function,
                Action = Common.Action.From(transaction.Action),
                SmartContractResult = Common.SmartContractResult.From(transaction.Results),
                EGLDPrice = transaction.Price,
                Logs = Log.From(transaction.Logs),
                Operations = Operation.From(transaction.Operations)
            };
        }

        /// <summary>
        /// Creates a new array of Transactions from data
        /// </summary>
        /// <param name="transactions">Array of Transaction Data Objects from API</param>
        /// <returns>Array of Transaction objects</returns>
        public static Transaction[] From(TransactionDto[] transactions)
        {
            return transactions.Select(transaction => new Transaction()
            {
                TxHash = transaction.TxHash,
                GasLimit = new GasLimit(transaction.GasLimit),
                GasPrice = transaction.GasPrice,
                GasUsed = new GasLimit(transaction.GasUsed),
                MiniBlockHash = transaction.MiniBlockHash,
                Nonce = transaction.Nonce,
                Receiver = Address.FromBech32(transaction.Receiver),
                ReceiverAssets = Assets.From(transaction.ReceiverAssets),
                ReceiverShard = transaction.ReceiverShard,
                Round = transaction.Round,
                Sender = Address.FromBech32(transaction.Sender),
                SenderAssets = Assets.From(transaction.SenderAssets),
                SenderShard = transaction.SenderShard,
                Signature = transaction.Signature,
                Status = transaction.Status,
                Value = ESDTAmount.From(transaction.Value),
                Fee = ESDTAmount.From(transaction.Fee),
                CreationDate = transaction.Timestamp.ToDateTime(),
                Data = DataCoder.DecodeData(transaction.Data),
                Function = transaction.Function,
                Action = Common.Action.From(transaction.Action),
                SmartContractResult = Common.SmartContractResult.From(transaction.Results),
                EGLDPrice = transaction.Price,
                Logs = Log.From(transaction.Logs),
                Operations = Operation.From(transaction.Operations)
            }).ToArray();
        }

        /// <summary>
        /// Synchronizes the transaction fields with the ones queried from the Network
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <returns></returns>
        public async Task Sync(IApiProvider provider)
        {
            var transaction = await provider.GetTransaction(TxHash);

            GasLimit = new GasLimit(transaction.GasLimit);
            GasPrice = transaction.GasPrice;
            GasUsed = new GasLimit(transaction.GasUsed);
            MiniBlockHash = transaction.MiniBlockHash;
            Nonce = transaction.Nonce;
            Receiver = Address.FromBech32(transaction.Receiver);
            ReceiverAssets = Assets.From(transaction.ReceiverAssets);
            ReceiverShard = transaction.ReceiverShard;
            Round = transaction.Round;
            Sender = Address.FromBech32(transaction.Sender);
            SenderAssets = Assets.From(transaction.SenderAssets);
            SenderShard = transaction.SenderShard;
            Signature = transaction.Signature;
            Status = transaction.Status;
            Value = ESDTAmount.From(transaction.Value);
            Fee = ESDTAmount.From(transaction.Fee ?? "0");
            CreationDate = transaction.Timestamp.ToDateTime();
            Data = DataCoder.DecodeData(transaction.Data);
            Function = transaction.Function;
            Action = Common.Action.From(transaction.Action);
            SmartContractResult = Common.SmartContractResult.From(transaction.Results);
            EGLDPrice = transaction.Price;
            Logs = Log.From(transaction.Logs);
            Operations = Operation.From(transaction.Operations);
        }

        /// <summary>
        /// Gets a parameter from smart contract results
        /// </summary>
        /// <typeparam name="T">Custom type</typeparam>
        /// <param name="type">Decode to this type</param>
        /// <param name="smartContractIndex">Index in the array of results</param>
        /// <param name="parameterIndex">Parameter index</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T GetSmartContractResult<T>(TypeValue type, int smartContractIndex = 0, int parameterIndex = 0) where T : IBinaryType
        {
            if (!SmartContractResult.Any())
                throw new Exception("Empty smart contract results");

            var scResult = SmartContractResult[smartContractIndex].Data;

            var fields = scResult.Split('@').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var result = fields.ElementAt(parameterIndex);
            var responseBytes = Converter.FromHexString(result);
            var binaryCodec = new BinaryCodec();
            var decodedResponse = binaryCodec.DecodeTopLevel(responseBytes, type);
            return (T)decodedResponse;
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
            return IsSuccessful() || IsFailed() || IsInvalid();
        }

        /// <summary>
        /// Returns whether the transaction has been executed successfully.
        /// </summary>
        /// <returns></returns>
        public bool IsSuccessful()
        {
            return Status == "success";
        }

        /// <summary>
        /// Returns whether the transaction has been executed, but with a failure.
        /// </summary>
        /// <returns></returns>
        public bool IsFailed()
        {
            return Status == "fail";
        }

        /// <summary>
        /// Returns whether the transaction has been executed, but marked as invalid (e.g. due to "insufficient funds")
        /// </summary>
        /// <returns></returns>
        public bool IsInvalid()
        {
            return Status == "invalid";
        }

        /// <summary>
        /// Wait for the execution of the transaction
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <param name="msCheck">Time interval to sync transaction status. Default: 1 second</param>
        /// <param name="timeout">Time interval until transaction check timeout. Default: 60 seconds</param>
        /// <returns></returns>
        /// <exception cref="TransactionException.TransactionStatusNotReachedException">Transaction timeout reached</exception>
        /// <exception cref="TransactionException.TransactionWithSmartContractErrorException">Transaction has Smart Contract Results error</exception>
        /// <exception cref="TransactionException.FailedTransactionException">Transaction is failed</exception>
        /// <exception cref="TransactionException.InvalidTransactionException">Transaction is invalid</exception>
        public async Task AwaitExecuted(IApiProvider provider, TimeSpan? msCheck = null, TimeSpan? timeout = null)
        {
            if (!msCheck.HasValue)
                msCheck = TimeSpan.FromSeconds(1);

            if (!timeout.HasValue)
                timeout = TimeSpan.FromSeconds(60);

            var secondsPassed = 0d;
            do
            {
                await Task.Delay(msCheck.Value);
                await Sync(provider);
                secondsPassed += msCheck.Value.TotalSeconds;
            } while (!IsExecuted() && secondsPassed < timeout.Value.TotalSeconds);

            if (!IsExecuted())
                throw new TransactionException.TransactionStatusNotReachedException(TxHash, "Executed");

            if (SmartContractResult != null && SmartContractResult.Any(s => !string.IsNullOrEmpty(s.ReturnMessage)))
            {
                var returnMessages = SmartContractResult.Select(x => x.ReturnMessage).ToArray();
                var aggregateMessage = string.Join(Environment.NewLine, returnMessages);
                throw new TransactionException.TransactionWithSmartContractErrorException(TxHash, aggregateMessage);
            }

            if (IsFailed())
                throw new TransactionException.FailedTransactionException(TxHash);

            if (IsInvalid())
                throw new TransactionException.InvalidTransactionException(TxHash);
        }

        /// <summary>
        /// Check if transactions is successful. To be used only after AwaitExecuted function
        /// </summary>
        /// <exception cref="TransactionException.InvalidTransactionException">Transactions is not successfully executed</exception>
        public void EnsureTransactionSuccess()
        {
            if (!IsSuccessful())
                throw new TransactionException.InvalidTransactionException(TxHash);
        }

        /// <summary>
        /// Wait for the transaction to be notarized
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <param name="msCheck">Time interval to check if transaction is notarized. Default: 1 second</param>
        /// <param name="timeout">Time interval until transaction notarization check timeout. Default: 60 seconds</param>
        /// <returns></returns>
        /// <exception cref="TransactionException.TransactionStatusNotReachedException">Transaction notarized timeout is reached</exception>
        public async Task AwaitNotarized(IApiProvider provider, TimeSpan? msCheck = null, TimeSpan? timeout = null)
        {
            if (!msCheck.HasValue)
                msCheck = TimeSpan.FromSeconds(1);

            if (!timeout.HasValue)
                timeout = TimeSpan.FromSeconds(60);

            var secondsPassed = 0d;
            do
            {
                await Task.Delay(msCheck.Value);
                await Sync(provider);
                secondsPassed += msCheck.Value.TotalSeconds;
            } while (string.IsNullOrEmpty(MiniBlockHash) && secondsPassed < timeout.Value.TotalSeconds);

            if (secondsPassed >= timeout.Value.TotalSeconds)
                throw new TransactionException.TransactionStatusNotReachedException(TxHash, "Notarized");
        }
    }
}
