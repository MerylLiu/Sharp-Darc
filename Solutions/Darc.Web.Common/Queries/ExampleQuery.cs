namespace Darc.Web.Common.Queries
{
    using System.Collections.Generic;
    using Dapper;

    public class ExampleQuery : DapperQuery, IExampleQuery
    {
        public List<string> GetQueries()
        {
            var data = new List<string>();
            data.Add("item1");
            data.Add("item2");
            data.Add("item3");
            data.Add("item4");

            return data;
        }
    }
}