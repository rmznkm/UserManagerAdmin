using Common.Events;
using ManagementAPI.Model;

namespace ManagementAPI.BusinessServices
{
    public interface IUserApprovalBusinessSerivce
    {
        Task AddUserRegisterRequestAsync(UserRegisterWaitingApprovalEvent userRegisterWaitingApproval);

        Task ApproveRequestAsync(Guid userRegisterRequestId);

        Task<IEnumerable<ApproveWaitingUser>> GetApproveWaitingUsersAsync();
    }
}
