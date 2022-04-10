using EventBus.Storage;
using FluentAssertions;
using ManagementAPI.Repositories;
using Moq;
using System.Data;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace ManagementAPI.Tests.Repositories
{
    public class UserRepositoryTests : TestsFor<UserRepository>
    {
        [Fact]
        public async Task AddUserRegisterRequestAsync_Every_CallDbStorage()
        {
            //Arrange & Act
            await Instance.AddUserRegisterRequestAsync(Guid.NewGuid(), new Models.UserRegisterRequest());

            //Assert
            GetMockFor<IDbStorage>().Verify(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()));
        }

        [Fact]
        public async Task UpdateAsApprovedAsync_Every_CallDbStorage()
        {
            //Arrange & Act
            await Instance.UpdateAsApprovedAsync(Guid.NewGuid());

            //Assert
            GetMockFor<IDbStorage>().Verify(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()));
        }


        [Fact]
        public async Task GetApproveWaitingUsersAsync_Every_CallDbStorage()
        {
            //Arrange
            var columnNames = new List<string> { "id", "first_name", "last_name", "updated_at" };
            var dataset = new List<object[]> { new object[] { Guid.NewGuid(), "FIRST_NAME", "LAST_NAME", DateTime.UtcNow } };
            var mockDataReader = new MockDataReader(columnNames, dataset);

            GetMockFor<IDbStorage>()
              .Setup(r => r.ExecuteReaderAsync(It.IsAny<string>(), It.IsAny<object>()))
              .Returns(() => Task.FromResult<IDataReader>(mockDataReader));

            //Act
            var result = await Instance.GetApproveWaitingUsersAsync();

            //Assert
            GetMockFor<IDbStorage>().Verify(x => x.ExecuteReaderAsync(It.IsAny<string>(), It.IsAny<object>()));
            result.Count().Should().Be(1);
        }
    }
}
