using ManagementAPI.Model;
using ManagementAPI.Models;

namespace ManagementAPI.Repositories
{
    public interface IUserRepository
    {
        Task AddUserRegisterRequestAsync(Guid userRegisterRequestId, UserRegisterRequest userRegisterRequest);
        Task UpdateAsApprovedAsync(Guid userRegisterRequestId);
        Task<IEnumerable<ApproveWaitingUser>> GetApproveWaitingUsersAsync();
    }
}
