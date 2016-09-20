namespace Darc.Core.Contracts
{
    using System.Data;

    public interface IDataSession
    {
        IDbConnection GetConnection();
    }
}