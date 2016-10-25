namespace Darc.Core.Attributes
{
    using System;
    using System.Runtime.Remoting.Messaging;

    public class DataSourceAttribute : Attribute
    {
        public DataSourceAttribute(string dataSource)
        {
            DataSource = dataSource;
            CallContext.LogicalSetData("$DataSource", dataSource);
        }

        public string DataSource { get; set; }
    }
}