using EventBus.Event;
using ManagementAPI.Models;

namespace Common.Events
{
    public class UserRegisterWaitingApprovalEvent : IEvent<UserRegisterRequest>
    {
        public Guid Id { get; set; }
        public UserRegisterRequest Model { get; set; }
    }
}
