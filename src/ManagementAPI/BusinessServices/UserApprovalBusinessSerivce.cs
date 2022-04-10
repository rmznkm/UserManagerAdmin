using Common.Events;
using ManagementAPI.Model;
using ManagementAPI.Repositories;

namespace ManagementAPI.BusinessServices
{
    public class UserApprovalBusinessSerivce : IUserApprovalBusinessSerivce
    {
        private readonly IUserRepository userRepository;
        public UserApprovalBusinessSerivce(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public Task AddUserRegisterRequestAsync(UserRegisterWaitingApprovalEvent userRegisterWaitingApproval)
        {
            return userRepository.AddUserRegisterRequestAsync(userRegisterWaitingApproval.Id, userRegisterWaitingApproval.Model);
        }

        public Task ApproveRequestAsync(Guid userRegisterRequestId)
        {
            return userRepository.UpdateAsApprovedAsync(userRegisterRequestId);
        }

        public Task<IEnumerable<ApproveWaitingUser>> GetApproveWaitingUsersAsync()
        {
            return userRepository.GetApproveWaitingUsersAsync();
        }
    }
}
