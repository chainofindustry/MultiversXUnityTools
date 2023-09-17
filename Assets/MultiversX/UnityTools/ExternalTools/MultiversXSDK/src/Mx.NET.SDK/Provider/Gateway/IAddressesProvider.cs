using Mx.NET.SDK.Provider.Dtos.Gateway.Addresses;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.Gateway
{
    public interface IAddressesProvider
    {
        /// <summary>
        /// This endpoint allows one to retrieve basic information about an Addresses (Account).
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns><see cref="AddressDto"/></returns>
        Task<AddressDataDto> GetAddress(string address);

        /// <summary>
        /// This endpoint allows one to retrieve the guardian data of an Address.
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <returns><see cref="AddressGuardianDataDto"/></returns>
        Task<AddressGuardianDataDto> GetAddressGuardianData(string address);

        /// <summary>
        /// This endpoint allows one to retrieve a value stored within the Blockchain for a given Address.
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <param name="key">Storage Key</param>
        /// <param name="isHex">Is hexadecimal encoded string</param>
        /// <returns></returns>
        Task<StorageValueDto> GetStorageValue(string address, string key, bool isHex = false);

        /// <summary>
        /// This endpoint allows one to retrieve all the key-value pairs stored under a given account.
        /// </summary>
        /// <param name="address">Wallet address in bech32 format</param>
        /// <returns></returns>
        Task<AllStorageDto> GetAllStorageValues(string address);
    }
}
