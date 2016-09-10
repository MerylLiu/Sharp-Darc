namespace Darc.Core.Data
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
            var connectionStrings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (!string.IsNullOrEmpty(connectionStrings.ProviderName))
                ProviderName = connectionStrings.ProviderName;
            else
                throw new Exception("The ProviderName is not in ConnectionStrings.");

            _dbFactory = DbProviderFactories.GetFactory(ProviderName);
            DbConnecttion = _dbFactory.CreateConnection();
            if (DbConnecttion != null)
            {
                DbConnecttion.ConnectionString = connectionStrings.ConnectionString;
                DbConnecttion.Open();
            }
            SetParamPrefix();
        }

        public IDbConnection DbConnecttion { get; }
        public string ParamPrefix { get; private set; } = "@";
        public string ProviderName { get; }
        public DbType DbType { get; private set; } = DbType.SqlServer;

        public void Dispose()
        {
            if (DbConnecttion != null)
            {
                try
                {
                    DbConnecttion.Dispose();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void SetParamPrefix()
        {
            var dbtype = (_dbFactory?.GetType() ?? DbConnecttion.GetType()).Name;

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

            if (DbType == DbType.MySql && DbConnecttion != null && DbConnecttion.ConnectionString != null)
                ParamPrefix = "?";
            if (DbType == DbType.Oracle)
                ParamPrefix = ":";
        }
    }
}