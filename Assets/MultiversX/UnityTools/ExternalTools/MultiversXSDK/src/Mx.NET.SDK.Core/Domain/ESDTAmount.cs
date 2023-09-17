using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Mx.NET.SDK.Core.Domain.Exceptions;

namespace Mx.NET.SDK.Core.Domain
{
    public class ESDTAmount
    {
        public ESDT Esdt { get; }
        public BigInteger Value { get; }

        private ESDTAmount(long value, ESDT token)
        {
            Esdt = token;
            Value = new BigInteger(value);
        }

        private ESDTAmount(string value, ESDT token)
        {
            Esdt = token;
            Value = BigInteger.Parse(value);
            if (Value.Sign == -1)
                throw new InvalidESDTAmountException(value);
        }

        public static bool operator >(ESDTAmount amount1, ESDTAmount amount2)
        {
            if (!amount1.Esdt.Equals(amount2.Esdt))
                throw new Exception($"Operator > cannot be applied between two different tokens");
            return amount1.Value > amount2.Value;
        }

        public static bool operator >=(ESDTAmount amount1, ESDTAmount amount2)
        {
            if (!amount1.Esdt.Equals(amount2.Esdt))
                throw new Exception($"Operator >= cannot be applied between two different tokens");
            return amount1.Value >= amount2.Value;
        }

        public static bool operator <(ESDTAmount amount1, ESDTAmount amount2)
        {
            if (!amount1.Esdt.Equals(amount2.Esdt))
                throw new Exception($"Operator < cannot be applied between two different tokens");
            return amount1.Value < amount2.Value;
        }

        public static bool operator <=(ESDTAmount amount1, ESDTAmount amount2)
        {
            if (!amount1.Esdt.Equals(amount2.Esdt))
                throw new Exception($"Operator <= cannot be applied between two different tokens");
            return amount1.Value <= amount2.Value;
        }

        public static bool operator ==(ESDTAmount amount1, ESDTAmount amount2)
        {
            if (!amount1.Esdt.Equals(amount2.Esdt))
                throw new Exception($"Operator == cannot be applied between two different tokens");
            return amount1.Value == amount2.Value;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
            {
                ESDTAmount amount = (ESDTAmount)obj;
                if (!Esdt.Equals(amount.Esdt))
                    throw new Exception($"Operator == cannot be applied between two different tokens");
                return Value == amount.Value;
            }
        }

        public static bool operator !=(ESDTAmount amount1, ESDTAmount amount2)
        {
            if (!amount1.Esdt.Equals(amount2.Esdt))
                throw new Exception($"Operator == cannot be applied between two different tokens");
            return amount1.Value != amount2.Value;
        }

        public static ESDTAmount operator +(ESDTAmount amount1, ESDTAmount amount2)
        {
            if (!amount1.Esdt.Equals(amount2.Esdt))
                throw new Exception($"Operator + cannot be applied between two different tokens");
            return new ESDTAmount($"{amount1.Value + amount2.Value}", amount1.Esdt);
        }

        public static ESDTAmount operator *(ESDTAmount tokenAmount, int multiplayer)
        {
            return new ESDTAmount($"{tokenAmount.Value * multiplayer}", tokenAmount.Esdt);
        }

        public static double operator /(ESDTAmount tokenAmount, double value)
        {
            return tokenAmount.ToDouble() / value;
        }

        public static double operator *(ESDTAmount tokenAmount, double value)
        {
            return tokenAmount.ToDouble() * value;
        }

        /// <summary>
        /// Returns the string representation of the value as Token currency.
        /// </summary>
        /// <returns></returns>
        public string ToCurrencyString()
        {
            var denominated = ToDenominated();
            return $"{denominated} {Esdt.Ticker}";
        }

        /// <summary>
        /// Returns the string representation of the value as Token currency.
        /// <param name="nrOfDecimals">Number of decimals to show</param>
        /// </summary>
        /// <returns></returns>
        public string ToCurrencyString(int nrOfDecimals)
        {
            var denominated = ToDenominated(nrOfDecimals);
            return $"{denominated} {Esdt.Ticker}";
        }

        /// <summary>
        /// String representation of the denominated value
        /// </summary>
        /// <returns></returns>
        public string ToDenominated()
        {
            var padded = Value.ToString().PadLeft(Esdt.DecimalPrecision, '0');

            var start = (padded.Length - Esdt.DecimalPrecision);
            start = start < 0 ? 0 : start;

            var decimals = padded.Substring(start, Esdt.DecimalPrecision);
            decimals = decimals.TrimEnd('0');
            var integer = start == 0 ? "0" : padded.Substring(0, start);

            if (string.IsNullOrEmpty(decimals))
                return integer;
            else
                return $"{integer}.{decimals}";
        }

        /// <summary>
        /// String representation of the denominated value
        /// </summary>
        /// <param name="nrOfDecimals">Number of decimals to show</param>
        /// <returns></returns>
        public string ToDenominated(int nrOfDecimals)
        {
            var padded = Value.ToString().PadLeft(Esdt.DecimalPrecision, '0');

            var start = (padded.Length - Esdt.DecimalPrecision);
            start = start < 0 ? 0 : start;

            var decimals = padded.Substring(start, Esdt.DecimalPrecision < nrOfDecimals ? Esdt.DecimalPrecision : nrOfDecimals);
            decimals = decimals.TrimEnd('0');
            var integer = start == 0 ? "0" : padded.Substring(0, start);

            if (string.IsNullOrEmpty(decimals))
                return integer;
            else
                return $"{integer}.{decimals}";
        }

        /// <summary>
        /// Double value of the denominated value
        /// </summary>
        /// <returns></returns>
        public double ToDouble()
        {
            return double.Parse(ToDenominated(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Creates a token amount object from an eGLD value (denomination will be applied).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static ESDTAmount EGLD(string value)
        {
            var egld = Domain.ESDT.EGLD();
            var split = value.Split('.');
            var integerPart = split.FirstOrDefault() ?? "0";
            var decimalPart = split.Length == 2 ? split[1] : string.Empty;
            var full = $"{integerPart}{decimalPart.PadRight(egld.DecimalPrecision, '0')}";
            return new ESDTAmount(full, Domain.ESDT.EGLD());
        }

        /// <summary>
        /// Create a token amount object from a value (denomination will be applied)
        /// </summary>
        /// <param name="value">Amount</param>
        /// <param name="token">Token, default is EGLD</param>
        /// <returns></returns>
        public static ESDTAmount ESDT(string value, ESDT token)
        {
            var split = value.Split('.');
            var integerPart = split.FirstOrDefault() ?? "0";
            var decimalPart = split.Length == 2 ? split[1] : string.Empty;
            var full = $"{integerPart}{decimalPart.PadRight(token.DecimalPrecision, '0')}";
            return new ESDTAmount(full, token);
        }

        public static ESDTAmount From(string value, ESDT token = null)
        {
            if (token == null)
                token = Domain.ESDT.EGLD();
            return new ESDTAmount(value, token);
        }

        public static ESDTAmount From(long value, ESDT token = null)
        {
            if (token == null)
                token = Domain.ESDT.EGLD();
            return new ESDTAmount(value, token);
        }

        /// <summary>
        /// Value zero
        /// </summary>
        /// <param name="token">Token, default is EGLD</param>
        /// <returns></returns>
        public static ESDTAmount Zero(ESDT token = null)
        {
            if (token == null)
                token = Domain.ESDT.EGLD();

            return new ESDTAmount(0, token);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
