namespace Darc.Tasks.Commands.Examples
{
    using System.ComponentModel.DataAnnotations;
    using Core;

    public class AddExampleCommand : CommandBase
    {
        public AddExampleCommand(string name)
        {
            Name = name;
        }

        [Required(ErrorMessage = "Please input the filed.")]
        public string Name { get; set; }

        public override TResult Handle<TResult>()
        {
            var data = "My test";
            return (TResult) (object) data;
        }
    }
}