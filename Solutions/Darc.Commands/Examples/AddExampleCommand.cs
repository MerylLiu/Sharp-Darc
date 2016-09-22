namespace Darc.Commands.Examples
{
    using Core.Helpers;
    using Dapper;
    using Domain;

    public class AddExampleCommand : BaseCommand
    {
        public AddExampleCommand(Example example)
        {
            Example = example;
        }

        public Example Example { get; set; }

        [Trans]
        public override object Handle<TResult>()
        {
            var data = DapperSession.Current.Save(Example);
            return data;
        }
    }
}