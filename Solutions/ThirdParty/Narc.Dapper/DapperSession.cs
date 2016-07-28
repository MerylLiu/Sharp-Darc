namespace Narc.Dapper
{
    using System.Configuration;
    using System.Data.Common;
    using System.Data.SqlClient;

    public class DapperSession
    {
        private static readonly string ConnectionStr = ConfigurationManager.ConnectionStrings["Narc"].ConnectionString;

        public static DbConnection GetConnection()
        {
            var connection = new SqlConnection(ConnectionStr);
            connection.Open();

            return connection;
        }
    }
}