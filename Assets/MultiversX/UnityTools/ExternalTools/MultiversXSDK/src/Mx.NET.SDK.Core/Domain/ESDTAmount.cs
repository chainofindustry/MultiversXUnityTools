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

        private ESDTAmount(decimal value, ESDT token)
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
        /// Create ESDT amount object from a value (denomination will be applied)
        /// </summary>
        /// <param name="value">ESDT amount</param>
        /// <param name="esdt">ESDT token, default is EGLD</param>
        /// <returns></returns>
        public static ESDTAmount ESDT(string value, ESDT esdt = null)
        {
            if (esdt is null) esdt = Domain.ESDT.EGLD();
            var split = value.Split('.');
            var integerPart = split.FirstOrDefault() ?? "0";
            var decimalPart = split.Length == 2 ? split[1] : string.Empty;
            var full = $"{integerPart}{decimalPart.PadRight(esdt.DecimalPrecision, '0')}";
            return new ESDTAmount(full, esdt);
        }

        /// <summary>
        /// Create ESDT amount object from a value (denomination will be applied)
        /// </summary>
        /// <param name="value">ESDT amount</param>
        /// <param name="esdt">ESDT Token, default is EGLD</param>
        /// <returns></returns>
        public static ESDTAmount ESDT(decimal value, ESDT esdt = null)
        {
            return ESDT(value.ToString(), esdt);
        }

        /// <summary>
        /// Creates ESDT amount object from eGLD value (denomination will be applied).
        /// </summary>
        /// <param name="value">eGLD value</param>
        /// <returns></returns>
        public static ESDTAmount EGLD(decimal value)
        {
            return ESDT(value);
        }

        /// <summary>
        /// Creates ESDT amount object from eGLD value (denomination will be applied).
        /// </summary>
        /// <param name="value">eGLD value</param>
        /// <returns></returns>
        public static ESDTAmount EGLD(string value)
        {
            return ESDT(value);
        }

        /// <summary>
        /// Creates ESDT amount object from a value
        /// </summary>
        /// <param name="value">ESDT Amount</param>
        /// <param name="esdt">ESDT Token, default is EGLD</param>
        /// <returns></returns>
        public static ESDTAmount From(BigInteger value, ESDT esdt = null)
        {
            return From(value.ToString(), esdt);
        }

        /// <summary>
        /// Creates ESDT amount object from a value
        /// </summary>
        /// <param name="value">ESDT Amount</param>
        /// <param name="esdt">ESDT Token, default is EGLD</param>
        /// <returns></returns>
        public static ESDTAmount From(string value, ESDT esdt = null)
        {
            return new ESDTAmount(value, esdt ?? Domain.ESDT.EGLD());
        }

        /// <summary>
        /// Value zero
        /// </summary>
        /// <param name="esdt">EGLD Token, default is EGLD</param>
        /// <returns></returns>
        public static ESDTAmount Zero(ESDT esdt = null)
        {
            return new ESDTAmount(0, esdt ?? Domain.ESDT.EGLD());
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
