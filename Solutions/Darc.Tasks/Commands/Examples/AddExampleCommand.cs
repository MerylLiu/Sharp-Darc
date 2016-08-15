namespace Darc.Tasks.Commands.Examples
{
    using System.ComponentModel.DataAnnotations;
    using Domain;

    public class AddExampleCommand : CommandBase
    {
        public AddExampleCommand(string field)
        {
            Field = field;
        }

        [Required(ErrorMessage = "Please input the filed.")]
        public string Field { get; set; }

        public override TResult Handle<TResult>()
        {
            var data = "My test";
            return (TResult)(object)data;
        }

        /*public override void Handle()
        {
            var data = "My test";
        }*/
    }
}