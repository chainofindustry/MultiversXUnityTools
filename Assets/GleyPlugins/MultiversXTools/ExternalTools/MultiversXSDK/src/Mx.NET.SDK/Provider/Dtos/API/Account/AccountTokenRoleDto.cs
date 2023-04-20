using System.Numerics;
using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Provider.Dtos.API.Account
{
    public class AccountTokenRoleDto
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Owner { get; set; }
        public int Decimals { get; set; }
        public bool IsPaused { get; set; }
        public dynamic Assets { get; set; } //JSON data
        public BigInteger Transactions { get; set; } = BigInteger.MinusOne;
        public BigInteger Accounts { get; set; } = BigInteger.MinusOne;
        public bool CanFreeze { get; set; }
        public bool CanWipe { get; set; }
        public bool CanPause { get; set; }
        public bool CanMint { get; set; }
        public bool CanBurn { get; set; }
        public bool CanChangeOwner { get; set; }
        public bool CanUpgrade { get; set; }
        public bool? CanAddSpecialRoles { get; set; }
        public TokenAccountRoleDto Role { get; set; }
    }
}
