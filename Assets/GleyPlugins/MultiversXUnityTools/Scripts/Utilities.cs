using Erdcsharp.Domain;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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

    public class Utilities
    {
        public static OperationResult IsNumberValid(ref string amount)
        {
            if (string.IsNullOrEmpty(amount))
            {
                return new OperationResult(OperationStatus.Error, $"Amount cannot be null");
            }
            amount = amount.Replace(",", ".");
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
