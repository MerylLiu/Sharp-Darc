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

        [Required(ErrorMessage = "Must be input the name")]
        public string Name { get; set; }
    }
}