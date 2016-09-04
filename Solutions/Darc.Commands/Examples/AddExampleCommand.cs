namespace Darc.Commands.Examples
{
    using Core;
    using Dapper;
    using Domain;

    public class AddExampleCommand : CommandBase
    {
        public AddExampleCommand(Example example)
        {
            Example = example;
        }

        public Example Example { get; set; }

        public override object Handle<TResult>()
        {
            var data = DapperSession.Current.Save(Example);
            return data;
        }
    }
}