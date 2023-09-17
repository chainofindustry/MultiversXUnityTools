using System.Linq;
using Mx.NET.SDK.Domain.Data.Properties;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.API.Common;

namespace Mx.NET.SDK.Domain.Data.Common
{
    /// <summary>
    /// /accounts/{address}/roles/collections | /{collection} endpoints
    /// </summary>
    public class CollectionAccountRole
    {
        public bool CanCreate { get; private set; }
        public bool CanBurn { get; private set; }
        public bool CanAddQuantity { get; private set; }
        public bool CanUpdateAttributes { get; private set; }
        public bool CanAddUri { get; private set; }
        public bool CanTransfer { get; private set; }
        public string[] Roles { get; private set; }

        private CollectionAccountRole() { }

        public static CollectionAccountRole From(CollectionAccountRoleDto role)
        {
            if (role == null) return null;

            return new CollectionAccountRole()
            {
                CanCreate = role.CanCreate,
                CanBurn = role.CanBurn,
                CanAddQuantity = role.CanAddQuantity,
                CanUpdateAttributes = role.CanUpdateAttributes,
                CanTransfer = role.CanTransfer,
                CanAddUri = role.CanAddUri,
                Roles = role.Roles
            };
        }
    }

    /// <summary>
    /// /collections all endpoints
    /// </summary>
    public class CollectionRoles
    {
        public Address Address { get; private set; }
        public bool CanCreate { get; private set; }
        public bool CanBurn { get; private set; }
        public bool CanAddQuantity { get; private set; }
        public bool CanUpdateAttributes { get; private set; }
        public bool CanAddUri { get; private set; }
        public bool CanTransfer { get; private set; }
        public string[] Roles { get; private set; }

        private CollectionRoles() { }

        public static CollectionRoles[] From(CollectionRolesDto[] roles)
        {
            if (roles == null) return null;

            return roles.Select(role => new CollectionRoles()
            {
                Address = Address.FromBech32(role.Address),
                CanCreate = role.CanCreate,
                CanBurn = role.CanBurn,
                CanAddQuantity = role.CanAddQuantity,
                CanUpdateAttributes = role.CanUpdateAttributes,
                CanAddUri = role.CanAddUri,
                CanTransfer = role.CanTransfer,
                Roles = role.Roles
            }).ToArray();
        }
    }
}
