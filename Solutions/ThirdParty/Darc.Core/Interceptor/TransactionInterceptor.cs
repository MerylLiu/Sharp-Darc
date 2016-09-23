namespace Darc.Core.Interceptor
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using Extensions;
    using Contracts;
    using Entities;
    using global::Castle.Core.Internal;
    using global::Castle.DynamicProxy;
    using Microsoft.Practices.ServiceLocation;

    public class TransactionInterceptor : IInterceptor
    {
        #region Implementation of IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;

            var attributes = invocation.Method.GetAttributes<TransAttribute>();
            var invocationAttr = invocation.MethodInvocationTarget.GetAttributes<TransAttribute>();

            var dataSourceAttr = invocation.TargetType.GetAttributes<DataSourceAttribute>();
            dataSourceAttr = dataSourceAttr.Any()
                ? dataSourceAttr
                : invocation.TargetType.BaseType.GetAttributes<DataSourceAttribute>();
            var dataSource = dataSourceAttr.Select(p => p.DataSource).FirstOrDefault();
            CallContext.LogicalSetData("$DataSource", dataSource);

            if (methodName.StartsWith("Do") || attributes.Any() || invocationAttr.Any())
            {
                var session = ServiceLocator.Current.GetInstance<IDataContext>();

                using (var conn = session.GetConnection(dataSource))
                {
                    var trans = conn.BeginTransaction();

                    CallContext.LogicalSetData("$DataSession", new DataSessionItems
                    {
                        Connection = conn,
                        Transaction = trans
                    });

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