using System;

namespace Mx.NET.SDK.Core.Domain.Exceptions
{
    public class InvalidESDTAmountException : Exception
    {
        public InvalidESDTAmountException(string value)
            : base($"Invalid TokenAmount {value}") { }
    }
}
