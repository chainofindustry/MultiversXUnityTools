using System.Threading.Tasks;

namespace MultiversX.UnityTools
{
    public interface IUnityProvider
    {
        Task<TR> Get<TR>(string requestUri);
        Task<TR> Post<TR>(string requestUri, object requestContent);
    }
}