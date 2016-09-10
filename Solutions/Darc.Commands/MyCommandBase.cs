namespace Darc.Commands
{
    using Core;
    using Dapper;
    using Dapper.Common;

    public class MyCommandBase : CommandBase
    {
        public virtual DapperSession Session => DapperSession.Current;
    }
}