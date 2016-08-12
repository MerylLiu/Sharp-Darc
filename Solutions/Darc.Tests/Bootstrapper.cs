namespace Darc.Tests
{
    using NUnit.Framework;

    [SetUpFixture]
    public class Bootstrapper
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            ServiceLocatorInitializer.Init();
        }
    }
}