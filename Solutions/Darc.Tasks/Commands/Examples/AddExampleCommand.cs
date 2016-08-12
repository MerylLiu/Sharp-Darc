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
    }
}