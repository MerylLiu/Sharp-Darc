namespace Narc.Tests
{
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Web.Common.Queries;

    [TestFixture]
    [Category("Test")]
    public class Test
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
            var ademo = ServiceLocator.Current.GetInstance<IExampleQuery>();
            var data = ademo.GetQueries();

            Assert.IsNotNull(data);
        }
    }
}