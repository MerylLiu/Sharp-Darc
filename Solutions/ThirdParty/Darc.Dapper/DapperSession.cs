namespace Darc.Dapper
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Runtime.Remoting.Messaging;
    using Common;
    using Core.Entities;

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

        public DapperSession(IDbConnection connection, IDbTransaction trans = null)
        {
            _context = new DbContext(connection, trans);
        }

        public static DapperSession Current
        {
            get
            {
                var session = CallContext.LogicalGetData("$DataSession") as DataSessionItems;
                if (session != null)
                    return new DapperSession(session.Connection, session.Transaction);

                var newDataSource = CallContext.LogicalGetData("$DataSource") as string;

                return !string.IsNullOrEmpty(newDataSource)
                    ? new DapperSession(newDataSource)
                    : new DapperSession(DataSource);
            }
        }
    }
}