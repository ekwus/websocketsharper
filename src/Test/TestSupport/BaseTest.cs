using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TestSupport
{
    public abstract class BaseTest
    {
        protected ILogger LOG { get; private set; }
        protected ILoggerProvider LoggerProvider { get; private set; }

        static BaseTest()
        {

        }

        public BaseTest(ITestOutputHelper testOutputHelper)
        {
            LoggerProvider = new xUnitLoggerProvider(testOutputHelper);
            LOG = LoggerProvider.CreateLogger("Unit Test");
        }
    }
}
