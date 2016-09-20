namespace Darc.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using Commands.Examples;
    using Core;
    using Domain;
    using Infrastructure.Utilities;
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
    }
}