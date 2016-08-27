namespace Darc.Domain
{
    using Common;
    using Dapper.Attributes;

    [Table("Mytest")]
    public class Example : Entity
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}