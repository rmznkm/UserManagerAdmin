using EventBus.Configuration.Storage;
using EventBus.Storage;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventBus.Tests.Configuration
{
    public class StorageTests
    {
        [Fact]
        public void AddStorage_Register()
        {
            // Act
            var services = new ServiceCollection();
            services.AddStorage();

            // Assert
            services.IsRegisted<IDbStorage>().Should().BeTrue();
        }
    }
}
