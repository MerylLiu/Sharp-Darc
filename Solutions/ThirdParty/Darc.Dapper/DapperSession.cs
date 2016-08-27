namespace Darc.Dapper
{
    using Common;

    public class DapperSession
    {
        private static readonly DbContext DbContext = new DbContext("Darc");
        public static DapperContext Current => new DapperContext(DbContext);
    }
}