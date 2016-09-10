namespace Darc.Core.Data
{
    public class DataSession
    {
        private static readonly DbContext DbContext = new DbContext("Darc");
        public virtual DataContext Current { get; set; } = new DataContext(DbContext);
    }
}