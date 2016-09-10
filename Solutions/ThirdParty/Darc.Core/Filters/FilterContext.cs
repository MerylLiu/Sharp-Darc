namespace Darc.Core.Filters
{
    using System;
    using System.Runtime.Remoting.Contexts;
    using System.Runtime.Remoting.Messaging;

    public class FilterContext : ContextAttribute, IContributeObjectSink
    {
        #region Implementation of IContributeObjectSink

        public FilterContext(string name) : base(name)
        {
        }

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            return new FilterHandler(nextSink);
        }

        #endregion
    }
}