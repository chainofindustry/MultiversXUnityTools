using System;

namespace MultiversXUnityTools
{
    public class NullDataException : Exception
    {
        public NullDataException(string code, string message)
             : base($"Null data received {code} - {message}")
        {
        }
    }
}