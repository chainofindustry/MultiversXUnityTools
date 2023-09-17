using System;
using System.Net;

namespace Mx.NET.SDK.Domain.Exceptions
{
    public class APIExceptionResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = "";
        public string Error { get; set; } = "";

        public APIExceptionResponse() { }
    }

    public class APIException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Error { get; set; }

        public APIException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.BadRequest;
            Error = "API error";
        }

        public APIException(APIExceptionResponse apiResponse) : base(apiResponse.Message)
        {
            StatusCode = apiResponse.StatusCode;
            Error = apiResponse.Error;
        }
    }
}
