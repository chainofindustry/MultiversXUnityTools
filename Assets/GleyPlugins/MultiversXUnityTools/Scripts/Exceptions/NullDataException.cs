using System;
namespace MultiversXUnityTools
{

    public class NullDataException : Exception
    {
        public NullDataException(string code, string message)
             : base($"Null date received {code} - {message}")
        {

        }
    }
}