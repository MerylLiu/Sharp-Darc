namespace Darc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands.Examples;
    using Core;
    using Core.Utilities;
    using Dapper;
    using Domain;
    using global::Dapper;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;

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

        private string ToBlock(Example data)
        {
            var res = $"The result is :\n{JsonUtil.JsonSerialize(data)}";
            return res;
        }

        private string ToBlock(IEnumerable<Example> data)
        {
            var res = $"The result is :\n{JsonUtil.JsonSerialize(data)}";
            return res;
        }

        [Test]
        public void Add()
        {
            var random = new Random().Next(1, 100);
            var data = new Example
            {
                Age = random,
                Name = $"INSERT({random})"
            };

            var res = DapperSession.Current.Save(data);

            Console.WriteLine(ToBlock(res));
            Assert.IsNotNull(res);
        }

        [Test]
        public void CommandProcessor()
        {
            var commandProcessor = ServiceLocator.Current.GetInstance<ICommandProcessor>();
            var command = new AddExampleCommand(new Example
            {
                Age = new Random().Next(0, 100),
                Name = "Test command handler"
            });

            var res = commandProcessor.Process<Example>(command);

            Console.WriteLine(ToBlock(res));
            Assert.IsNotNull(res);
        }

        [Test]
        public void CommandProcessorWithHandler()
        {
            var commandProcessor = ServiceLocator.Current.GetInstance<ICommandProcessor>();
            var command = new AddExampleCommand(new Example
            {
                Age = new Random().Next(0, 100)
            });

            commandProcessor.Process<Example>(command, p =>
            {
                //Do something with the p(Example)
            });
        }

        [Test]
        public void Delete()
        {
            var res = DapperSession.Current.Delete<Example>(p => p.Id == (object) 2);

            Console.WriteLine($"Delete is {(res ? "successful" : "failed")}");
            Assert.AreEqual(true, res);
        }

        [Test]
        public void ExecuteSql()
        {
            var rows = DapperSession.Current.Execute("delete from mytest where id = ?Id", new { Id = 20 });

            Console.WriteLine($"Execute is {(rows > 0 ? "successful" : "failed")}");
            Assert.AreEqual(true, rows > 0);
        }

        [Test]
        public void ExecuteSqlQuery()
        {
            var res = DapperSession.Current.Query<Example>("select * from mytest where id=:Id", new {Id = 3});

            Console.WriteLine(ToBlock(res));
            Assert.IsNotNull(res);
        }

        [Test]
        public void GetAll()
        {
            var res = DapperSession.Current.All<Example>();

            Console.WriteLine(ToBlock(res));
            Assert.IsNotNull(res);
        }

        [Test]
        public void GetList()
        {
            var res = DapperSession.Current.Find<Example>(p => p.Age == 111);

            Console.WriteLine(ToBlock(res));
            Assert.IsNotNull(res);
        }

        [Test]
        public void GetSingle()
        {
            var res = DapperSession.Current.Get<Example>(39);

            Console.WriteLine(ToBlock(res));
            Assert.IsNotNull(res);
        }

        [Test]
        public void Update()
        {
            var random = new Random().Next(1, 100);
            var data = new Example
            {
                Id = 40,
                Age = random,
                Name = $"Update({random})"
            };

            var res = DapperSession.Current.Update(data);

            Console.WriteLine($"Update is {(res ? "successful" : "failed")}");
            Assert.AreEqual(true, res);
        }
    }
}