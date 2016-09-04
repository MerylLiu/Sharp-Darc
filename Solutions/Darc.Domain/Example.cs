namespace Darc.Domain
{
    using Common;
    using Dapper.Attributes;
    using System.ComponentModel.DataAnnotations;

    [Table("Mytest"),Sequence("example_seq")]
    public class Example : Entity
    {
        [Required(ErrorMessage = "Please input the name.")]
        public string Name { get; set; }

        public int Age { get; set; }
    }
}