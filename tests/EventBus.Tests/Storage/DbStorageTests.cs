using EventBus.Storage;
using FluentAssertions;
using Moq;
using System.Data;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.Storage
{
    public class DbStorageTests : TestsFor<DbStorage>
    {
        [Fact]
        public void GetStorageConnection_JustOnce_CallConnection()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.GetStorageConnection();
            Instance.GetStorageConnection();

            //Assert
            GetMockFor<IDbConnectionProvider>().Verify(x => x.Connection, Times.Once());
        }

        [Fact]
        public void IsInTransaction_IfTranscationNotStarted_ReturnFalse()
        {
            //Arrange & Act
            var result = Instance.InTransaction();

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BeginTransaction_Every_CallConnetionBeginTransaction()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.BeginTransaction();

            //Assert
            mockConnection.Verify(x => x.BeginTransaction());
        }

        [Fact]
        public void Open_IfNotOpened_CallConnetionOpen()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            mockConnection
                .Setup(x => x.State)
                .Returns(ConnectionState.Closed);

            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.Open();

            //Assert
            mockConnection.Verify(x => x.Open());
        }

        [Fact]
        public void Close_IfOpened_CallConnetionClose()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            mockConnection
                .Setup(x => x.State)
                .Returns(ConnectionState.Open);

            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.Close();

            //Assert
            mockConnection.Verify(x => x.Close());
        }

        [Fact]
        public void CommitTransaction_IfNoTransaction_ThrowException()
        {
            //Arrange && Act &&Assert
            Assert.Throws<InvalidOperationException>(() => Instance.CommitTransaction());
        }

        [Fact]
        public void CommitTransaction_IfHasTransaction_CallTransactionCommit()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();
            mockConnection
                .Setup(x => x.BeginTransaction())
                .Returns(mockTransaction.Object);

            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.BeginTransaction();
            Instance.CommitTransaction();

            //Assert
            mockTransaction.Verify(x => x.Commit());
        }

        [Fact]
        public void RollbackTransaction_IfNoTransaction_ThrowException()
        {
            //Arrange && Act &&Assert
            Assert.Throws<InvalidOperationException>(() => Instance.RollbackTransaction());
        }

        [Fact]
        public void RollbackTransaction_IfHasTransaction_CallTransactionRollback()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();
            mockConnection
                .Setup(x => x.BeginTransaction())
                .Returns(mockTransaction.Object);

            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.BeginTransaction();
            Instance.RollbackTransaction();

            //Assert
            mockTransaction.Verify(x => x.Rollback());
        }

        [Fact]
        public void GetStorageTransaction_Every_ReturnCurrentTransaction()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();
            mockConnection
                .Setup(x => x.BeginTransaction())
                .Returns(mockTransaction.Object);

            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.BeginTransaction();
            var transaction = Instance.GetStorageTransaction();

            //Assert
            transaction.Should().Be(mockTransaction.Object);
        }

        [Fact]
        public void Dispose_IfHas_CallTransactionDisposeConnectionDispose()
        {
            //Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();
            mockConnection
                .Setup(x => x.BeginTransaction())
                .Returns(mockTransaction.Object);

            GetMockFor<IDbConnectionProvider>()
                .Setup(x => x.Connection)
                .Returns(mockConnection.Object);

            //Act
            Instance.BeginTransaction();
            Instance.Dispose();

            //Assert
            mockConnection.Verify(x => x.Dispose());
            mockTransaction.Verify(x => x.Dispose());
        }
    }
}