using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Provider.Dtos.API.Transactions
{
    public class TransactionDto
    {
        public string TxHash { get; set; }
        public long GasLimit { get; set; }
        public long GasPrice { get; set; }
        public long GasUsed { get; set; }
        public string MiniBlockHash { get; set; }
        public long Nonce { get; set; }
        public string Receiver { get; set; }
        public AssetsDto ReceiverAssets { get; set; }
        public long ReceiverShard { get; set; }
        public long Round { get; set; }
        public string Sender { get; set; }
        public AssetsDto SenderAssets { get; set; }
        public long SenderShard { get; set; }
        public string Signature { get; set; }
        public string Status { get; set; }
        public string Value { get; set; }
        public string Fee { get; set; }
        public long Timestamp { get; set; }
        public string Data { get; set; }
        public string Function { get; set; }
        public ActionDto Action { get; set; }
        public SmartContractResultDto[] Results { get; set; }
        public string Price { get; set; }
        public LogDto Logs { get; set; }
        public OperationDto[] Operations { get; set; }
    }

    public class ActionDto
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ArgumentDto Arguments { get; set; }
    }

    public class ArgumentDto
    {
        public TransferDto[] Transfers { get; set; }
        public string Receiver { get; set; }
        public string FunctionName { get; set; }
        public string[] FunctionArgs { get; set; }
        public AssetsDto ReceiverAssets { get; set; }
    }

    public class TransferDto
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string SvgUrl { get; set; }
        public string Collection { get; set; }
        public string Token { get; set; }
        public int? Decimals { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }
    }

    public class SmartContractResultDto
    {
        public string Hash { get; set; }
        public long Timestamp { get; set; }
        public long Nonce { get; set; }
        public long GasLimit { get; set; }
        public long GasPrice { get; set; }
        public string Value { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public AssetsDto SenderAssets { get; set; }
        public AssetsDto ReceiverAssets { get; set; }
        public string Data { get; set; }
        public string PrevTxHash { get; set; }
        public string OriginalTxHash { get; set; }
        public string CallType { get; set; }
        public string ReturnMessage { get; set; }
        public string Operation { get; set; }
        public string Function { get; set; }
    }

    public class LogDto
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public AssetsDto AddressAssets { get; set; }
        public EventDto[] Events { get; set; }
    }

    public class EventDto
    {
        public string Identifier { get; set; }
        public string Address { get; set; }
        public string Data { get; set; }
        public string[] Topics { get; set; }
        public int? Order { get; set; }
        public AssetsDto AddressAssets { get; set; }
    }

    public class OperationDto
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }
        public string EsdtType { get; set; }
        public string Collection { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Data { get; set; }
        public string Value { get; set; }
        public int? Decimals { get; set; }
        public string SvgUrl { get; set; }
        public AssetsDto SenderAssets { get; set; }
        public AssetsDto ReceiverAssets { get; set; }
        public string Message { get; set; }
    }
}
