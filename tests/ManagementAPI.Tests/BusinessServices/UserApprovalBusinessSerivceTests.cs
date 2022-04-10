using Common.Events;
using FizzWare.NBuilder;
using FluentAssertions;
using ManagementAPI.BusinessServices;
using ManagementAPI.Model;
using ManagementAPI.Models;
using ManagementAPI.Repositories;
using Moq;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace ManagementAPI.Tests.BusinessServices
{
    public class UserApprovalBusinessSerivceTests : TestsFor<UserApprovalBusinessSerivce>
    {
        [Fact]
        public async Task AddUserRegisterRequestAsync_Every_CallUserRepository()
        {
            //Arrange & Act
            await Instance.AddUserRegisterRequestAsync(new UserRegisterWaitingApprovalEvent());

            //Assert
            GetMockFor<IUserRepository>().Verify(x => x.AddUserRegisterRequestAsync(It.IsAny<Guid>(), It.IsAny<UserRegisterRequest>()));
        }

        [Fact]
        public async Task ApproveRequestAsync_Every_CallUserRepository()
        {
            //Arrange & Act
            await Instance.ApproveRequestAsync(Guid.NewGuid());

            //Assert
            GetMockFor<IUserRepository>().Verify(x => x.UpdateAsApprovedAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public async Task GetApproveWaitingUsersAsync_Count_EqualRepositoryCount()
        {
            //Arrange
            IEnumerable<ApproveWaitingUser> approveWaitingUsers = Builder<ApproveWaitingUser>
              .CreateListOfSize(5)
              .Build();


            GetMockFor<IUserRepository>()
                .Setup(x => x.GetApproveWaitingUsersAsync())
                .Returns(Task.FromResult(approveWaitingUsers));

            //Act
            var result = await Instance.GetApproveWaitingUsersAsync();

            //Assert
            result.Count().Should().Be(approveWaitingUsers.Count());
        }
    }
}