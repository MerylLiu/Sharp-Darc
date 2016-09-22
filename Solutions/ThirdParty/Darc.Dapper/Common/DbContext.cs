namespace Darc.Dapper.Common
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;

    public class DbContext : IDisposable
    {
        private readonly DbProviderFactory _dbFactory;

        public DbContext(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (!string.IsNullOrEmpty(connectionString.ProviderName))
                ProviderName = connectionString.ProviderName;
            else
                throw new Exception("The ProviderName is not in ConnectionStrings.");

            _dbFactory = DbProviderFactories.GetFactory(ProviderName);
            DbConnection = _dbFactory.CreateConnection();
            if (DbConnection != null)
            {
                DbConnection.ConnectionString = connectionString.ConnectionString;
                DbConnection.Open();
            }
            SetParamPrefix();
        }

        public DbContext(IDbConnection connection, IDbTransaction transaction = null)
        {
            _dbFactory = DbProviderFactories.GetFactory((DbConnection)connection);
            DbConnection = connection;

            if (DbConnection != null && DbConnection.State == ConnectionState.Closed) DbConnection.Open();

            DbTransaction = transaction;
            SetParamPrefix();
        }

        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }
        public string ParamPrefix { get; private set; } = "@";
        public string ProviderName { get; }
        public DbType DbType { get; private set; } = DbType.SqlServer;

        public void Dispose()
        {
            if (DbConnection != null)
            {
                try
                {
                    DbConnection.Dispose();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void SetParamPrefix()
        {
            var dbtype = (_dbFactory?.GetType() ?? DbConnection.GetType()).Name;

            if (dbtype.StartsWith("MySql")) DbType = DbType.MySql;
            else if (dbtype.StartsWith("SqlCe")) DbType = DbType.SqlServerCe;
            else if (dbtype.StartsWith("Npgsql")) DbType = DbType.PostgreSql;
            else if (dbtype.StartsWith("Oracle")) DbType = DbType.Oracle;
            else if (dbtype.StartsWith("SQLite")) DbType = DbType.SQLite;
            else if (dbtype.StartsWith("System.Data.SqlClient")) DbType = DbType.SqlServer;
            // else try with provider name
            else if (ProviderName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                DbType = DbType.MySql;
            else if (ProviderName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0)
                DbType = DbType.SqlServerCe;
            else if (ProviderName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                DbType = DbType.PostgreSql;
            else if (ProviderName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0)
                DbType = DbType.Oracle;
            else if (ProviderName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0)
                DbType = DbType.SQLite;

            if (DbType == DbType.MySql && DbConnection != null && DbConnection.ConnectionString != null)
                ParamPrefix = "?";
            if (DbType == DbType.Oracle)
                ParamPrefix = ":";
        }
    }
}