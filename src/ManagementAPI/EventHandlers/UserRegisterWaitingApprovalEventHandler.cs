using Common.Events;
using EventBus.Event;
using ManagementAPI.BusinessServices;

namespace ManagementAPI.EventHandlers
{
    public class UserRegisterWaitingApprovalEventHandler : IEventHandler<UserRegisterWaitingApprovalEvent>
    {
        private readonly IUserApprovalBusinessSerivce userApprovalSerivce;
        public UserRegisterWaitingApprovalEventHandler(IUserApprovalBusinessSerivce userApprovalSerivce)
        {
            this.userApprovalSerivce = userApprovalSerivce;
        }
        public Task HandleAsync(UserRegisterWaitingApprovalEvent @event)
        {
            return userApprovalSerivce.AddUserRegisterRequestAsync(@event);
        }
    }
}
