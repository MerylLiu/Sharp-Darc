namespace Darc.Commands
{
    using Core;
    using Core.Helpers;
    using Dapper;

    public class BaseCommand : CommandBase
    {
        public virtual DapperSession Session => DapperSession.Current;
    }
}