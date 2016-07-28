namespace Narc.Web.Areas.Admin.Conrtollers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Common;
    using Common.Queries;
    using Domain;
    using Framework.Extensions;
    using Framework.Utilities;
    using Tasks.Commands.Examples;

    public class ExampleController : Controller
    {
        public IExampleQuery ExampleQuery { get; set; }
        public ICommandProcessor CommandProcessor { get; set; }

        public ActionResult Query()
        {
            List<string> data = ExampleQuery.GetQueries();

            return View(data);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(ExampleModel example)
        {
            IList<string> messages = new List<string>();

            Try.CatchBiz(() =>
                {
                    var command = new AddExampleCommand(example.Field);

                    CommandProcessor.Process(command);
                },
                         bex => { messages = bex.ErrorMessages; },
                         ex => messages.Add("异常错误。"));

            if (messages.Count == 0)
            {
                return Json(new {result = true});
            }
            return Json(new {result = false, message = messages.ToHtml()});
        }
    }
}