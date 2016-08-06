namespace Narc.Domain
{
    using Dos.ORM;

    public class MyTest : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}