namespace Darc.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;

    public class ExampleQuery : BaseQuery, IExampleQuery
    {
        public IList<Example> GetQueries()
        {
            var data = Session.All<Example>().OrderBy(p => p.Id).ToList();
            return data;
        }
    }
}