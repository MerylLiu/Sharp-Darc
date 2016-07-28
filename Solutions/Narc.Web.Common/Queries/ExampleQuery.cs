namespace Narc.Web.Common.Queries
{
    using System.Collections.Generic;
    using Dapper;

    public class ExampleQuery : DapperQuery, IExampleQuery
    {
        public List<string> GetQueries()
        {
            throw new System.NotImplementedException();
        }
    }
}