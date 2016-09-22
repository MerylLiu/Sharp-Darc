namespace Darc.Dapper.Common
{
    using System.Data;
    using Core.Contracts;

    public class DataContext : IDataContext
    {
        #region Implementation of IDataSession

        public IDbConnection GetConnection(string dataSource = null)
        {
            return dataSource == null
                ? new DbContext(DapperSession.DataSource).DbConnection
                : new DbContext(dataSource).DbConnection;
        }

        #endregion
    }
}