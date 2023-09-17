using Mx.NET.SDK.Core.Domain.Constants;
using Mx.NET.SDK.Core.Domain.Exceptions;
using Mx.NET.SDK.Core.Domain.Helper;
using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Mx.NET.SDK.Core.Domain
{
    public class ESDT
    {
        private readonly Regex _nameValidation = new Regex("^[a-zA-Z0-9]{3,20}$");
        private readonly Regex _tickerValidation = new Regex("^[A-Z0-9]{3,10}$");
        public string Name { get; }
        public string Identifier { get; }
        public string Collection { get; }
        public string Ticker { get; }
        public int DecimalPrecision { get; }

        public ESDT(string name, string identifier, string collection, string ticker, int decimalPrecision)
        {
            //if (!_nameValidation.IsMatch(name))
            //throw new ArgumentException("Length should be between 3 and 20 characters, alphanumeric characters only", nameof(name));
            Name = name;

            Identifier = identifier;
            Collection = collection;

            //if (!_tickerValidation.IsMatch(ticker))
            //throw new ArgumentException("Length should be between 3 and 10 characters, alphanumeric UPPERCASE characters only", nameof(ticker));
            Ticker = ticker;

            if (decimalPrecision < 0 || decimalPrecision > 18)
                throw new ArgumentException("Should be between 0 and 18", nameof(decimalPrecision));
            DecimalPrecision = decimalPrecision;

        }

        /// <summary>
        /// MultiversX eGold token (EGLD)
        /// </summary>
        /// <returns></returns>
        public static ESDT EGLD()
        {
            return new ESDT("EGLD", Constants.Constants.EGLD, Constants.Constants.EGLD, Constants.Constants.EGLD, 18);
        }

        public static ESDT ESDT_TOKEN(string esdtType, string name, string identifier, int decimalPrecision)
        {
            switch (esdtType)
            {
                case ESDTTokenType.EGLD:
                    return EGLD();
                case ESDTTokenType.FungibleESDT:
                    return TOKEN(name, identifier, decimalPrecision);
                case ESDTTokenType.NonFungibleESDT:
                case ESDTTokenType.SemiFungibleESDT:
                    return NFT(name, identifier);
                case ESDTTokenType.MetaESDT:
                    return META_ESDT(name, identifier, decimalPrecision);
                default:
                    throw new InvalidESDTTypeException(esdtType);
            }
        }

        /// <summary>
        /// Create an ESDT Token
        /// </summary>
        /// <param name="name">The name of the token</param>
        /// <param name="identifier">The token identifier (e.g. ABC-123456)</param>
        /// <param name="decimalPrecision">Decimal precision of the token (max 18)</param>
        /// <returns></returns>
        public static ESDT TOKEN(string name, string identifier, int decimalPrecision)
        {
            return new ESDT(name, identifier, identifier, identifier.GetTicker(), decimalPrecision);
        }

        /// <summary>
        /// Create an ESDT NFT Token
        /// </summary>
        /// <param name="name">The name of the token</param>
        /// <param name="identifier">The token identifier (e.g. NFT-123456-01)</param>
        /// <returns></returns>
        public static ESDT NFT(string name, string identifier)
        {
            return new ESDT(name, identifier, identifier.GetCollection(), identifier.GetTicker(), 0);
        }

        /// <summary>
        /// Create an META ESDT Token
        /// </summary>
        /// <param name="name">The name of the token</param>
        /// <param name="identifier">The token identifier (e.g. META-123456-01)</param>
        /// <returns></returns>
        public static ESDT META_ESDT(string name, string identifier, int decimalPrecision)
        {
            return new ESDT(name, identifier, identifier.GetCollection(), identifier.GetTicker(), decimalPrecision);
        }

        /// <summary>
        /// The value One
        /// </summary>
        /// <returns></returns>
        public BigInteger One()
        {
            var value = "1".PadRight(DecimalPrecision + 1, '0');
            return BigInteger.Parse(value);
        }

        /// <summary>
        /// The value Zero
        /// </summary>
        /// <returns></returns>
        public BigInteger Zero()
        {
            return new BigInteger(0);
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// ESDT Identifiers are equal
        /// </summary>
        /// <param name="esdt"></param>
        /// <returns></returns>
        public bool Equals(ESDT esdt)
        {
            return Identifier == esdt.Identifier;
        }
    }
}
