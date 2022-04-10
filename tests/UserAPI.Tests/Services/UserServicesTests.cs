using Common.Events;
using EventBus.Transport;
using Moq;
using UserAPI.Services;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace UserAPI.Tests.Services
{
    public class UserServicesTests : TestsFor<UserServices>
    {
        [Fact]
        public async void RegisterAsync_Every_CallTransport()
        {
            //Arrange & Act
           await Instance.RegisterAsync(new Models.UserRegisterRequest());

            //Assert
            GetMockFor<ITransport>().Verify(x => x.Publish(It.IsAny<UserRegisterWaitingApprovalEvent>()));
        }
    }
}
