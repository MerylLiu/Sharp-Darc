namespace Narc.Web.Controllers
{
    using System.Web.Mvc;
    using Common.Queries;

    public class HomeController : Controller
    {
        public IExampleQuery ExampleQuery { get; set; }

        public ActionResult Index()
        {
            var data = ExampleQuery.GetQueries();
            return View(data);
        }
    }
}