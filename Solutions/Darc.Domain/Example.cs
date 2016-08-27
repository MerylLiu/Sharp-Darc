namespace Darc.Domain
{
    using Common;
    using Dapper.Attributes;

    [Table("Mytest"),Sequence("example_seq")]
    public class Example : Entity
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}