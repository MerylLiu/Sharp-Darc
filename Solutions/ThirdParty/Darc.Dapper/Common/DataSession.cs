namespace Darc.Dapper.Common
{
    using System.Data;
    using Core.Contracts;

    public class DataSession : IDataSession
    {
        #region Implementation of IDataSession

        public IDbConnection GetConnection()
        {
            return new DbContext(DapperSession.DataSource).DbConnection;
        }

        #endregion
    }
}