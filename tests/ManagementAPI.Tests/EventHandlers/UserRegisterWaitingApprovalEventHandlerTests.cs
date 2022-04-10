using Common.Events;
using ManagementAPI.BusinessServices;
using ManagementAPI.EventHandlers;
using Moq;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace ManagementAPI.Tests.EventHandlers
{
    public class UserRegisterWaitingApprovalEventHandlerTests : TestsFor<UserRegisterWaitingApprovalEventHandler>
    {
        [Fact]
        public async void HandleAsync_Every_CallUserApprovalSerivce()
        {
            //Arrange & Act
            await Instance.HandleAsync(new UserRegisterWaitingApprovalEvent());

            //Assert
            GetMockFor<IUserApprovalBusinessSerivce>().Verify(x => x.AddUserRegisterRequestAsync(It.IsAny<UserRegisterWaitingApprovalEvent>()));
        }
    }
}
