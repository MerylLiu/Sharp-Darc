namespace Darc.Dapper
{
    using System;
    using System.Configuration;
    using Common;

    public partial class DapperSession
    {
        private static DbContext _context;
        internal static readonly string DataSource = ConfigurationManager.AppSettings["DefaultDataSouce"];
        public static readonly Lazy<DapperSession> Lazy = new Lazy<DapperSession>(() => new DapperSession(DataSource));

        public DapperSession(DbContext context)
        {
            _context = context;
        }

        public DapperSession(string dataSouce)
        {
            var dbContext = new DbContext(dataSouce);
            _context = dbContext;
        }

        public static DapperSession Current => Lazy.Value;
    }
}