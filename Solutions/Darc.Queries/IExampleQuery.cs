namespace Darc.Queries
{
    using System.Collections.Generic;
    using Domain;

    public interface IExampleQuery
    {
        IList<Example> GetQueries();

        /// <summary>
        ///     With transaction
        /// </summary>
        /// <returns></returns>
        IList<Example> DoGetQueries();
    }
}