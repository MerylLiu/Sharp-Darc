namespace Darc.Commands.Examples
{
    using System.ComponentModel.DataAnnotations;
    using Core;

    public class AddExample2Command : CommandBase
    {
        public AddExample2Command()
        {
        }

        public AddExample2Command(string name)
        {
            Name = name;
        }

        [Required(ErrorMessage = "The name is must be inputed")]
        public string Name { get; set; }
    }
}