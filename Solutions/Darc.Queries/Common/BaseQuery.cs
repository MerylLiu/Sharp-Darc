namespace Darc.Queries
{
    using Dapper;
    using Dapper.Common;

    public class BaseQuery : DapperQuery
    {
        public virtual DapperSession Session => DapperSession.Current;
    }
}