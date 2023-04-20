using System.Collections.Generic;

namespace Mx.NET.SDK.Provider.Dtos.API.Transactions
{
    public class MultipleTransactionsResponseDto
    {
        public int NumOfSentTxs { get; set; }
        public Dictionary<string, string> TxsHashes { get; set; }
    }
}
