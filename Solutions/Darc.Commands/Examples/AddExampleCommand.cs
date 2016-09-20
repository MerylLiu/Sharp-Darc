namespace Darc.Commands.Examples
{
    using Dapper;
    using Domain;

    public class AddExample : BaseCommand
    {
        public AddExample(Example example)
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