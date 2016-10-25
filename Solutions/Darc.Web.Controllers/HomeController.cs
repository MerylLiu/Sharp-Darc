namespace Darc.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Commands.Examples;
    using Core;
    using Core.Extensions;
    using Core.Utilities;
    using Domain;
    using Queries;

    public class HomeController : Controller
    {
        public IExampleQuery ExampleQuery { get; set; }
        public ICommandProcessor CommandProcessor { get; set; }

        public ActionResult Index()
        {
            var data = ExampleQuery.DoGetQueries();

            LogUtil.Log<HomeController>().Error("Log4net test.");

            return View(data);
        }

        public ActionResult Add()
        {
            IList<string> messages = new List<string>();

            Try.CatchBiz(() =>
            {
                var command = new AddExampleCommand(new Example
                {
                    Age = new Random().Next(0, 100),
                    Name = "Test command handler"
                });

                var res = CommandProcessor.Process<Example>(command);
            },
                bex => { messages = bex.ErrorMessages; },
                ex => messages.Add("异常错误。"));

            if (messages.Count == 0)
            {
                return Json(new {result = true}, JsonRequestBehavior.AllowGet);
            }
            return Json(new {result = false, message = messages.ToHtml()}, JsonRequestBehavior.AllowGet);
        }
    }
}