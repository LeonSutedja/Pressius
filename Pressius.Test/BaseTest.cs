using Xunit.Abstractions;

namespace Pressius.Test
{
    public abstract class BaseTest
    {
        protected readonly ITestOutputHelper _output;

        protected BaseTest(ITestOutputHelper output)
        {
            _output = output;
        }
    }
}