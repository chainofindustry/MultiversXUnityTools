using Mx.NET.SDK.Provider.Dtos.API.Account;
using System.Threading.Tasks;

namespace Mx.NET.SDK.Provider.API
{
    public interface IUsernamesProvider
    {
        /// <summary>
        /// Returns the bech32 address base on the username
        /// </summary>
        /// <param name="username">Account username</param>
        /// <returns></returns>
        Task<AccountDto> GetAccountByUsername(string username);
    }
}
