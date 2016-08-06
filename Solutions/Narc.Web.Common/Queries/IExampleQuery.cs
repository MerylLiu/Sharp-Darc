namespace Narc.Web.Common.Queries
{
    using System.Collections.Generic;
    using Domain;

    public interface IExampleQuery
    {
        List<MyTest> GetQueries();
    }
}