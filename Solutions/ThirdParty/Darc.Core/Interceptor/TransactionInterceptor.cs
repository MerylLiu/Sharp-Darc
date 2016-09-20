namespace Darc.Core.Interceptor
{
    using System;
    using Contracts;
    using Exceptions;
    using global::Castle.DynamicProxy;
    using Microsoft.Practices.ServiceLocation;

    public class TransactionInterceptor : IInterceptor
    {
        #region Implementation of IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method.Name;

            if (method.StartsWith("Do"))
            {
                var session = ServiceLocator.Current.GetInstance<IDataSession>();

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