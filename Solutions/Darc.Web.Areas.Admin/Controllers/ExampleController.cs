﻿namespace Darc.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Commands.Examples;
    using Core;
    using Core.Extensions;
    using Core.Utilities;
    using Domain;
    using Queries;

    public class ExampleController : Controller
    {
        public IExampleQuery ExampleQuery { get; set; }
        public ICommandProcessor CommandProcessor { get; set; }

        public ActionResult Query()
        {
            var data = ExampleQuery.GetQueries();

            return View(data);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Example example)
        {
            IList<string> messages = new List<string>();

            Try.CatchBiz(() =>
            {
                var command = new AddExampleCommand(new Example());

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