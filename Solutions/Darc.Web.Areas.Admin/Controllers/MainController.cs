namespace Darc.Web.Areas.Admin.Controllers
{
    using System.Web.Mvc;

    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return Json(new {Id = "test"}, JsonRequestBehavior.AllowGet);
        }
    }
}