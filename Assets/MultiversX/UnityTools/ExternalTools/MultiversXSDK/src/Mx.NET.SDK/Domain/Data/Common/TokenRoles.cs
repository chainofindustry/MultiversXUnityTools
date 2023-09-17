using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Common;
using System.Linq;

namespace Mx.NET.SDK.Domain.Data.Common
{
    /// <summary>
    /// /accounts/{address}/roles/tokens endpoint
    /// </summary>
    public class TokenAccountRole
    {
        public bool CanLocalMint { get; private set; }
        public bool CanLocalBurn { get; private set; }
        public bool CanTransfer { get; private set; }
        public string[] Roles { get; private set; }

        private TokenAccountRole() { }

        public static TokenAccountRole From(TokenAccountRoleDto role)
        {
            if (role == null) return null;

            return new TokenAccountRole()
            {
                CanLocalMint = role.CanLocalMint,
                CanLocalBurn = role.CanLocalBurn,
                CanTransfer = role.CanTransfer,
                Roles = role.Roles
            };
        }
    }

    /// <summary>
    /// /tokens/{identifier} endpoint
    /// </summary>
    public class TokenRoles
    {
        public Address Address { get; private set; }
        public bool CanLocalMint { get; private set; }
        public bool CanLocalBurn { get; private set; }
        public bool CanTransfer { get; private set; }
        public string[] Roles { get; private set; }

        private TokenRoles() { }

        public static TokenRoles[] From(TokenRolesDto[] roles)
        {
            if (roles == null) return null;

            return roles.Select(role => new TokenRoles()
            {
                Address = Address.FromBech32(role.Address),
                CanLocalMint = role.CanLocalMint,
                CanLocalBurn = role.CanLocalBurn,
                CanTransfer = role.CanTransfer,
                Roles = role.Roles
            }).ToArray();
        }
    }
}
