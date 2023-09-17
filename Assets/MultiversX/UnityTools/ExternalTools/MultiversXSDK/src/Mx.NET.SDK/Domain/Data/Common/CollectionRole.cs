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
        public CollectionManagerProperties AddressProperties { get; private set; }
        public string[] Roles { get; private set; }

        private CollectionAccountRole() { }

        public static CollectionAccountRole From(CollectionAccountRoleDto role)
        {
            if (role == null) return null;

            return new CollectionAccountRole()
            {
                AddressProperties = CollectionManagerProperties.From(role.CanCreate,
                                                                     role.CanBurn,
                                                                     role.CanAddQuantity,
                                                                     role.CanUpdateAttributes,
                                                                     role.CanTransfer,
                                                                     role.CanAddUri),
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
        public CollectionManagerProperties AddressProperties { get; private set; }
        public string[] AddressRoles { get; private set; }

        private CollectionRoles() { }

        public static CollectionRoles[] From(CollectionRolesDto[] roles)
        {
            if (roles == null) return null;

            return roles.Select(role => new CollectionRoles()
            {
                Address = Address.FromBech32(role.Address),
                AddressProperties = CollectionManagerProperties.From(role.CanCreate,
                                                                     role.CanBurn,
                                                                     role.CanAddQuantity,
                                                                     role.CanUpdateAttributes,
                                                                     role.CanTransfer,
                                                                     role.CanAddUri),
                AddressRoles = role.Roles
            }).ToArray();
        }
    }
}
