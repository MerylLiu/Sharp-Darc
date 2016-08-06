namespace Narc.Infrastructure.DbContext
{
    using Dos.ORM;

    public class NaSession
    {
        public static readonly DbSession Context = new DbSession("Narc");
    }
}