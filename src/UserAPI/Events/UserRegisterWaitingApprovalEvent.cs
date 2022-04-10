using EventBus.Event;
using UserAPI.Models;

namespace Common.Events
{
    public class UserRegisterWaitingApprovalEvent : IEvent<UserRegisterRequest>
    {
        public Guid Id { get; set; }
        public UserRegisterRequest Model { get; set; }
    }
}
