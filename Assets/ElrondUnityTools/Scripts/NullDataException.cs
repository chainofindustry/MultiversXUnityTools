using System;
namespace ElrondUnityTools
{

    public class NullDataException : Exception
    {
        public NullDataException(string code, string message)
             : base($"Null date received {code} - {message}")
        {

        }
    }
}