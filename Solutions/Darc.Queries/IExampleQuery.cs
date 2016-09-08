namespace Darc.Queries
{
    using System.Collections.Generic;
    using Domain;

    public interface IExampleQuery
    {
        IList<Example> GetQueries();
    }
}