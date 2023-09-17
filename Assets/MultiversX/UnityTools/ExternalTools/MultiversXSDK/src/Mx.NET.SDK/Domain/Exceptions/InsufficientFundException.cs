using System;

namespace Mx.NET.SDK.Domain.Exceptions
{
    public class InsufficientFundException : Exception
    {
        public InsufficientFundException(string tokenIdentifier)
            : base($"Insufficient fund for token : {tokenIdentifier}") { }
    }
}
