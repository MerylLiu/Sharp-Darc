namespace Narc.Web.Common.Queries
{
    using System.Collections.Generic;
    using Domain;
    using Infrastructure.DbContext;

    public class ExampleQuery : CommonQuery, IExampleQuery
    {
        public List<MyTest> GetQueries()
        {
            var data = NaSession.Context.From<MyTest>().Select(d => new {d.Id, d.Name, d.Age}).ToList();
            return data;
        }
    }
}