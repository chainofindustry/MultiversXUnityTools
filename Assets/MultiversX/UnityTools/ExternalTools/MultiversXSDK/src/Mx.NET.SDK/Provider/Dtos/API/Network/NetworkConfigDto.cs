namespace Mx.NET.SDK.Provider.Dtos.API.Network
{
    public class NetworkConfigDto
    {
        public string ChainId { get; set; }
        public long GasPerDataByte { get; set; }
        public long MinGasLimit { get; set; }
        public long MinGasPrice { get; set; }
        public string GasPriceModifier { get; set; } = "0.01";
        public int MinTransactionVersion { get; set; }
    }
}
