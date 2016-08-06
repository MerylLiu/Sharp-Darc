namespace Narc.Tests
{
    using FS.Tests;
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