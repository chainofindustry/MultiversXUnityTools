using System;

namespace Mx.NET.SDK.Core.Domain.Exceptions
{
    public class WrongBinaryValueCodecException : Exception
    {
        public WrongBinaryValueCodecException()
            : base("Wrong binary argument") { }
    }
}
