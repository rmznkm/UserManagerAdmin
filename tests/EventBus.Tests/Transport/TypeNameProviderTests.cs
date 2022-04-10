using EventBus.Transport;
using FluentAssertions;
using UserManagerAdmin.TestHelper;
using Xunit;

namespace EventBus.Tests.Transport
{
    public class Foo { }

    public class TypeNameProviderTests : TestsFor<TypeNameProvider>
    {
        [Fact]
        public void GetTypeName_IfNull_ReturnNull()
        {
            // Act 
            var result = Instance.GetTypeName(null);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetTypeName_IfNotNull_ReturnNotNull()
        {
            // Act 
            var result = Instance.GetTypeName(typeof(Foo));

            //Assert
            result.Should().NotBeNull();
            result.Should().Be(typeof(Foo).FullName);
        }
    }
}
