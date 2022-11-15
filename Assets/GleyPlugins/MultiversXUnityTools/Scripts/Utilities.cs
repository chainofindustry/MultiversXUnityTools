using Erdcsharp.Domain;
using System.Text.RegularExpressions;

namespace MultiversXUnityTools
{
    public struct OperationResult
    {
        public OperationStatus status;
        public string message;

        public OperationResult(OperationStatus status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }

    /// <summary>
    /// Used to check the validity of different types of data
    /// </summary>
    public class Utilities
    {
        public static OperationResult IsNumberValid(ref string amount)
        {
            if (string.IsNullOrEmpty(amount))
            {
                return new OperationResult(OperationStatus.Error, $"Amount cannot be null");
            }
            //sdk requires . for decimals, input fields accept . & ,
            amount = amount.Replace(",", ".");

            //check if it is number
            if (!Regex.IsMatch(amount, "^\\d*\\.?\\d*$"))
            {
                return new OperationResult(OperationStatus.Error, $"Invalid amount: {amount}");
            }
            return new OperationResult(OperationStatus.Complete, null);
        }


        public static bool IsAddressValid(string erdAddress)
        {
            try
            {
                Address.From(erdAddress);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
