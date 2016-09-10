namespace Darc.Core.Filters
{
    using System;
    using System.Runtime.Remoting.Messaging;
    using Microsoft.Practices.ServiceLocation;

    public class FilterHandler : IMessageSink
    {
        public FilterHandler(IMessageSink nextSink)
        {
            NextSink = nextSink;
        }

        #region Implementation of IMessageSink

        public IMessage SyncProcessMessage(IMessage msg)
        {
            IMessage retMessage = null;
            var callMessage = msg as IMethodCallMessage;

            if (callMessage == null ||
                Attribute.GetCustomAttribute(callMessage.MethodBase, typeof (FilterAttribute)) == null)
            {
                retMessage = NextSink.SyncProcessMessage(msg);
            }
            else
            {
                var filter = ServiceLocator.Current.GetInstance<IFilterAttribute>();

                filter.OnExecuting();
                retMessage = NextSink.SyncProcessMessage(msg);
                filter.OnExecuted();
            }

            return retMessage;
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            throw new NotImplementedException();
        }

        public IMessageSink NextSink { get; }

        #endregion
    }
}