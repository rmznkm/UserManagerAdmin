using FizzWare.NBuilder;
using FluentAssertions;
using ManagementAPI.BusinessServices;
using ManagementAPI.Model;
using ManagementAPI.Services;
using Moq;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace ManagementAPI.Tests.Services
{
    public class UserApprovalServiceTests : TestsFor<UserApprovalService>
    {
        [Fact]
        public async void ApproveWaitingUser_Every_CallUserApprovalBusinesService()
        {
            //Arrange & Act
            await Instance.ApproveWaitingUser(new ApproveRequest {RequestId=Guid.NewGuid().ToString() }, null);

            //Assert
            GetMockFor<IUserApprovalBusinessSerivce>().Verify(x => x.ApproveRequestAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public async void GetApprovalWaitingUsers_Count_EqualUserApprovalBusinesServiceResponseCount()
        {
            //Arrange
            IEnumerable<ApproveWaitingUser> approveWaitingUsers = Builder<ApproveWaitingUser>
                .CreateListOfSize(5)
                .All()
                .With(c => c.UpdatedAt = DateTime.UtcNow)
                .Build();

            GetMockFor<IUserApprovalBusinessSerivce>()
                .Setup(x => x.GetApproveWaitingUsersAsync()).
                Returns(Task.FromResult(approveWaitingUsers));

            //Act
            var result = await Instance.GetApprovalWaitingUsers(null, null);

            //Assert
            result.Users.Count.Should().Be(approveWaitingUsers.Count());
        }
    }
}
