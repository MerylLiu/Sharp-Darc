namespace Darc.Core.Contracts
{
    using System.Data;

    public interface IDataContext
    {
        IDbConnection GetConnection(string dataSource = null);
    }
}