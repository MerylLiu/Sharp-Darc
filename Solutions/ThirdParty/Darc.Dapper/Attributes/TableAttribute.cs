namespace Darc.Dapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : BaseAttribute
    {
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}