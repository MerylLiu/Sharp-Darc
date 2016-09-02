namespace Darc.Web.Common.Queries
{
    using System.Collections.Generic;
    using Domain;

    public interface IExampleQuery
    {
        IList<Example> GetQueries();
    }
}