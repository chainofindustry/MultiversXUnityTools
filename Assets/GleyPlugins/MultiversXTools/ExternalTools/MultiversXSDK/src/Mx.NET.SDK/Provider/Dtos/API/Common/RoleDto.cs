namespace Mx.NET.SDK.Provider.Dtos.API.Common
{
    public class CollectionAccountRoleDto
    {
        public bool CanCreate { get; set; }
        public bool CanBurn { get; set; }
        public bool CanAddQuantity { get; set; }
        public bool CanUpdateAttributes { get; set; }
        public bool CanAddUri { get; set; }
        public bool CanTransfer { get; set; }
        public string[] Roles { get; set; }
    }

    public class CollectionRolesDto
    {
        public string Address { get; set; }
        public bool CanCreate { get; set; }
        public bool CanBurn { get; set; }
        public bool CanAddQuantity { get; set; }
        public bool CanUpdateAttributes { get; set; }
        public bool CanAddUri { get; set; }
        public bool CanTransfer { get; set; }
        public string[] Roles { get; set; }
    }

    public class TokenAccountRoleDto
    {
        public bool CanLocalMint { get; set; }
        public bool CanLocalBurn { get; set; }
    }
}
