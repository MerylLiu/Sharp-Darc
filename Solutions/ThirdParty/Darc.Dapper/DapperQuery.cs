namespace Darc.Dapper
{
    using Castle.Core;

    [Interceptor("Transaction")]
    public class DapperQuery
    {
        public virtual DapperSession Session => DapperSession.Current;
    }
}