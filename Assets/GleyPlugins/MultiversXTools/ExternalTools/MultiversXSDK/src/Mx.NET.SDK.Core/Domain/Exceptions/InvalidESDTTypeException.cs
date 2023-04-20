using System;

namespace Mx.NET.SDK.Core.Domain.Exceptions
{
    public class InvalidESDTTypeException : Exception
    {
        public InvalidESDTTypeException(string value)
            : base($"Invalid ESDT Type {value}") { }
    }
}
