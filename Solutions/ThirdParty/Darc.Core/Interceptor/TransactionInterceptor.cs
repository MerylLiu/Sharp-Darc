namespace Darc.Core.Interceptor
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using Contracts;
    using global::Castle.Core.Internal;
    using global::Castle.DynamicProxy;
    using Helpers;
    using Microsoft.Practices.ServiceLocation;

    public class TransactionInterceptor : IInterceptor
    {
        #region Implementation of IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;

            var attributes = invocation.Method.GetAttributes<TransAttribute>();
            var invocationAttributes = invocation.MethodInvocationTarget.GetAttributes<TransAttribute>();

            if (methodName.StartsWith("Do") || attributes.Any() || invocationAttributes.Any())
            {
                var session = ServiceLocator.Current.GetInstance<IDataSession>();
                CallContext.LogicalSetData("DataSession",session);

                using (var conn = session.GetConnection())
                {
                    var trans = conn.BeginTransaction();

                    try
                    {
                        invocation.Proceed();
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            else
            {
                invocation.Proceed();
            }

            #endregion
        }
    }
}