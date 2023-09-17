namespace Mx.NET.SDK.Provider.Dtos.API.Transactions
{
    public class TransactionResponseDto
    {
        public string Receiver { get; set; }
        public long ReceiverShard { get; set; }
        public string Sender { get; set; }
        public long SenderShard { get; set; }
        public string Status { get; set; }
        public string TxHash { get; set; }
    }
}
