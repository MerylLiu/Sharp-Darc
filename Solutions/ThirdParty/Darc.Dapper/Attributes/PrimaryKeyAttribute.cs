namespace Darc.Dapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PrimaryKeyAttribute : BaseAttribute
    {
        public PrimaryKeyAttribute()
        {
            AutoIncrement = false;
        }

        public PrimaryKeyAttribute(bool autoIncrement)
        {
            AutoIncrement = autoIncrement;
        }

        public bool AutoIncrement { get; set; }
    }
}