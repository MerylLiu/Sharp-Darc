namespace Darc.Web.Common.Queries
{
    using System.Collections.Generic;
    using Dapper;
    using Domain;

    public class ExampleQuery : DapperQuery, IExampleQuery
    {
        public IList<Example> GetQueries()
        {
            var data = DapperSession.Current.All<Example>();

            return data;
        }
    }
}