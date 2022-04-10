using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ManagementAPI.BusinessServices;

namespace ManagementAPI.Services
{
    public class UserApprovalService : UserApproval.UserApprovalBase
    {
        private readonly IUserApprovalBusinessSerivce userApprovalBusinesService;
        public UserApprovalService(IUserApprovalBusinessSerivce userApprovalBusinesService)
        {
            this.userApprovalBusinesService = userApprovalBusinesService;
        }

        public override async Task<ApproveReply> ApproveWaitingUser(ApproveRequest request, ServerCallContext context)
        {
            await userApprovalBusinesService.ApproveRequestAsync(new Guid(request.RequestId));
            return new ApproveReply { Message = "Approved" };
        }

        public override async Task<ApprovalWaitingUsers> GetApprovalWaitingUsers(ApprovalWaitingUserRequest request, ServerCallContext context)
        {
            var users = await userApprovalBusinesService.GetApproveWaitingUsersAsync();
            var response = new ApprovalWaitingUsers();
            foreach (var user in users)
            {
                response.Users.Add(new ApprovalWaitingUser
                {
                    RequestId = user.Id.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UpdatedAt = Timestamp.FromDateTime(user.UpdatedAt)
                });
            }
            return response;
        }
    }
}
