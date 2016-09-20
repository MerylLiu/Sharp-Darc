namespace Darc.Web.Controllers
{
    using System.Web.Mvc;
    using Infrastructure.Utilities;
    using Queries;

    public class HomeController : Controller
    {
        public IExampleQuery ExampleQuery { get; set; }

        public ActionResult Index()
        {
            var data = ExampleQuery.GetQueries();

            LogUtil.Log<HomeController>().Error("Log4net test.");

            return View(data);
        }
    }
}