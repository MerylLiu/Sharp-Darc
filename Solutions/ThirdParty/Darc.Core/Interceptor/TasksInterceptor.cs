namespace Darc.Core.Interceptor
{
    using global::Castle.DynamicProxy;

    public class TasksInterceptor : IInterceptor
    {
        #region Implementation of IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
        }

        #endregion
    }
}