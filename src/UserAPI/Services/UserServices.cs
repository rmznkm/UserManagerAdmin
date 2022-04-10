using Common.Events;
using EventBus.Transport;
using EventBus.Utilities;
using UserAPI.Models;
namespace UserAPI.Services
{
    public class UserServices : IUserServices
    {
        private readonly ITransport transport;

        public UserServices(ITransport transport)
        {
            this.transport = transport;
        }

        public Task<UserRegisterResponse> RegisterAsync(UserRegisterRequest userRegisterRequest)
        {
            var id = CombGuid.Generate();
            transport.Publish(new UserRegisterWaitingApprovalEvent { Model = userRegisterRequest, Id = id });
            return Task.FromResult(new UserRegisterResponse { IsSuccess = true, Message = "WaitingApproval" });
        }
    }
}
