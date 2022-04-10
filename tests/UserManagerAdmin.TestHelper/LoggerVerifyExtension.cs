using Microsoft.Extensions.Logging;
using Moq;

namespace UserManagerAdmin.TestHelper
{
    public static class LoggerVerifyExtension
    {
        public static void VerifyLoggger<T>(this Mock<ILogger> loggerMock, LogLevel logLevel, Times times) where T : Exception
        {
            loggerMock.Verify(l => l.Log(logLevel,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<T>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            times);
        }
    }
}
