namespace SharpStrc.Web.Common.Queries
{
    using System;
    using System.Collections.Generic;
    using Darc.Dapper;

    public class ExampleQuery : DapperQuery, IExampleQuery
    {
        public List<string> GetQueries()
        {
            throw new NotImplementedException();
        }
    }
}