namespace Darc.Core.Entities
{
    using System.Data;

    public class DataSessionItems
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
    }
}