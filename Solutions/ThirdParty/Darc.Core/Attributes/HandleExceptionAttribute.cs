namespace Darc.Core.Attributes
{
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using Exceptions;
    using Extensions;
    using Utilities;

    public class HandleExceptionAttribute : HandleErrorAttribute
    {
        #region Overrides of HandleErrorAttribute

        public override void OnException(ExceptionContext filterContext)
        {
            var globalException = ConfigurationManager.AppSettings["GlobalException"];

            if (!filterContext.ExceptionHandled && (globalException != null && globalException.ToLower() == "true"))
            {
                var controller = filterContext.Controller;
                string errorMsg;

                var ex = filterContext.Exception;

                if (ex.GetType() == typeof (BizException))
                {
                    var bex = ex as BizException;
                    errorMsg = bex.ErrorMessages.ToHtml();

                    var result = new {result = false, message = errorMsg};
                    filterContext.Result = new JsonResult
                    {
                        Data = result,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    errorMsg = ex.Message;

                    var result = new {result = false, message = "异常错误。"};
                    filterContext.Result = new JsonResult
                    {
                        Data = result,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }

                LogUtil.Log(controller.GetType()).Error(errorMsg);

                if (!filterContext.Controller.GetType()
                    .GetCustomAttributes(typeof (HttpPostAttribute), false).Any() )
                {
                    string controllerName = (string)filterContext.RouteData.Values["controller"];
                    string actionName = (string)filterContext.RouteData.Values["action"];
                    HandleErrorInfo info = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                    filterContext.Result = new ViewResult()
                    {
                        ViewName = "/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(info)
                    };
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                }

                filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }

        #endregion
    }
}