using Moq;
using UserAPI.Controllers;
using UserAPI.Models;
using UserAPI.Services;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace UserAPI.Tests.Controllers
{
    public class UserControllerTests : TestsFor<UserController>
    {
        [Fact]
        public async void RegisterAsync_Every_CallTransport()
        {
            //Arrange & Act
            await Instance.RegisterAsync(new UserRegisterRequest());

            //Assert
            GetMockFor<IUserServices>().Verify(x => x.RegisterAsync(It.IsAny<UserRegisterRequest>()));
        }
    }
}

