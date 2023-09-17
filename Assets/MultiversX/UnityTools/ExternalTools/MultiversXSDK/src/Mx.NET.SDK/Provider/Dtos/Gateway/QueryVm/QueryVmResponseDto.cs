namespace Mx.NET.SDK.Provider.Dtos.Gateway.Query
{
    public class QueryVmResponseDto
    {
        public QueryVmResponseDataDto Data { get; set; }
    }

    public class QueryVmResponseDataDto
    {
        public string[] ReturnData { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }
}
