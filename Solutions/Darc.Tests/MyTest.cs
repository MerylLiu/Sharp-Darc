namespace Darc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Dapper;
    using Domain;
    using global::Dapper;
    using Infrastructure.Utilities;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Commands.Examples;

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

        #region Serialize the result.

        private string ToBlock(Example data)
        {
            var res = $"The result is :\n{JsonUtil.JsonSerialize(data)}";
            return res;
        }

        private string ToBlock(IList<Example> data)
        {
            var res = $"The result is :\n{JsonUtil.JsonSerialize(data)}";
            return res;
        }

        #endregion

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
        public void Delete()
        {
            var res = DapperSession.Current.Delete<Example>(p => p.Id == (object)2);

            Console.WriteLine($"Delete is {(res ? "successful" : "failed")}");
            Assert.AreEqual(true, res);
        }

        [Test]
        public void ExecuteWithoutReturns()
        {
            DapperSession.Current.Call(p =>
            {
                var rows = p.Execute("delete from mytest where id = ?Id", new {Id = 40});

                Console.WriteLine($"Execute is {(rows > 0 ? "successful" : "failed")}");
                Assert.AreEqual(true, rows > 0);
            });
        }

        [Test]
        public void ExecuteWithReturns()
        {
            var res = DapperSession.Current.Call(p =>
            {
                var data = p.Query<Example>("select * from mytest").ToList();
                return data;
            });
            /*DapperSession.Current.ExecuteSql(p => p.Query<Example>("select * from mytest")
                .ToList());*/

            Console.WriteLine(ToBlock(res));
            Assert.IsNotNull(res);
        }

        [Test]
        public void ExecuteWithTransaction()
        {
            DapperSession.Current.Call((p, t) =>
            {
                var rows = p.Execute("delete from mytest where id = ?Id", new {Id = 40}, t);

                Console.WriteLine($"Execute is {(rows > 0 ? "successful" : "failed")}");
                Assert.AreEqual(true, rows > 0);
            });
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
        public void CommandProcessor()
        {
            var commandProcessor = ServiceLocator.Current.GetInstance<ICommandProcessor>();
            var command = new AddExample(new Example()
            {
                Age = new Random().Next(0,100),
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
            var command = new AddExample(new Example()
            {
                Age = new Random().Next(0,100),
            });

            commandProcessor.Process<Example>(command, p =>
            {
                //Do something with the p(Example)
            });
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