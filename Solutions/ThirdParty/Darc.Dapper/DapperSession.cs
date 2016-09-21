namespace Darc.Dapper
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Runtime.Remoting.Messaging;
    using Common;

    public partial class DapperSession
    {
        private static DbContext _context;
        internal static readonly string DataSource = ConfigurationManager.AppSettings["DefaultDataSouce"];
        private static readonly Lazy<DapperSession> Lazy = new Lazy<DapperSession>(() => new DapperSession(DataSource));

        public DapperSession(DbContext context)
        {
            _context = context;
        }

        public DapperSession(string dataSouce)
        {
            _context = new DbContext(dataSouce);
        }

        public DapperSession(IDbConnection connection)
        {
            _context = new DbContext(connection);
        }

        public static DapperSession Current
        {
            get
            {
                var session = CallContext.LogicalGetData("DataSession") as DataSession;
                if (session != null)
                {
                    var conn = session.GetConnection();
                    return new DapperSession(conn);
                }

                return Lazy.Value;
            }
        }
    }
}