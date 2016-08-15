namespace Darc.Tests
{
    using Domain;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Tasks.Commands.Examples;
    using Web.Common.Queries;

    [TestFixture]
    [Category("MyTest")]
    public class MyTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GetSingleADemo()
        {
            var commandProcessor = ServiceLocator.Current.GetInstance<ICommandProcessor>();
            var command = new AddExampleCommand("Test my handler");

            commandProcessor.Process<string>(command, (p) =>
            {
            });
        }
    }
}