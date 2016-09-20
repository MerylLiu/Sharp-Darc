using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darc.Infrastructure.Extensions
{
    using Castle.DynamicProxy;
    using Utilities;

    public class Logger:IInterceptor
    {
        #region Implementation of IInterceptor

        public void Intercept(IInvocation invocation)
        {
            LogUtil.Log<Logger>().Info("calling");
            invocation.Proceed();
            LogUtil.Log<Logger>().Info("finished");
        }

        #endregion
    }
}
