using System.Threading.Tasks;
using UserAPI.Models;

namespace UserAPI.Services
{
    public interface IUserServices
    {
        Task<UserRegisterResponse> RegisterAsync(UserRegisterRequest userRegisterRequest);
    }
}
