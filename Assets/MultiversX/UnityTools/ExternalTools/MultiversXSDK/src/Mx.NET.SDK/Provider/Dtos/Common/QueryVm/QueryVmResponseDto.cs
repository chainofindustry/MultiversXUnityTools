namespace Mx.NET.SDK.Provider.Dtos.Common.QueryVm
{
    public class QueryVmResponseDataDto
    {
        public QueryVmResponseDto Data { get; set; }
    }

    public class QueryVmResponseDto
    {
        public string[] ReturnData { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }
}
