using Mx.NET.SDK.Provider.Dtos.API.Common;
using System.Numerics;

namespace Mx.NET.SDK.Provider.Dtos.API.Tokens
{
    public class TokenDto
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Owner { get; set; }
        public string Minted { get; set; }
        public string Burnt { get; set; }
        public string InitialMinted { get; set; }
        public int Decimals { get; set; }
        public bool IsPaused { get; set; }
        public dynamic Assets { get; set; } //JSON data
        public BigInteger Transactions { get; set; } = BigInteger.MinusOne;
        public BigInteger Accounts { get; set; } = BigInteger.MinusOne;
        public bool CanUpgrade { get; set; }
        public bool CanMint { get; set; }
        public bool CanBurn { get; set; }
        public bool CanChangeOwner { get; set; }
        public bool CanAddSpecialRoles { get; set; }
        public bool CanPause { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanWipe { get; set; }
        public string Price { get; set; }
        public string MarketCap { get; set; }
        public string Supply { get; set; }
        public string CirculatingSupply { get; set; }
        public TokenRolesDto[] Roles { get; set; }
    }
}
