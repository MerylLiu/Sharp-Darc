namespace Darc.Queries
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Exceptions;
    using Domain;

    public class ExampleQuery : BaseQuery, IExampleQuery
    {
        public IList<Example> GetQueries()
        {
            var data = Session.All<Example>().OrderBy(p => p.Id).ToList();
            throw new BizException("ddd");
            return data;
        }

        public IList<Example> DoGetQueries()
        {
            var data = Session.All<Example>().OrderBy(p => p.Id).ToList();
            return data;
        }
    }
}