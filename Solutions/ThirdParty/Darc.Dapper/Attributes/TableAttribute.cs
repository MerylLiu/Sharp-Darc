namespace Darc.Dapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class TableAttribute : BaseAttribute
    {
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}