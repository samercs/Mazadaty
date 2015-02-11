using Mzayad.Web.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    public class DocsXController : ApplicationController
    {
        public DocsXController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        // GET: Docs
        public ActionResult Index()
        {
            var x = Environment.GetEnvironmentVariable("HOME");
            var y = Path.Combine(x, @"site\wwwdocs");
            var d = new DirectoryInfo(y);
            var files = d.GetFiles("*.md");

            var file = files.FirstOrDefault();
            if (file == null)
            {
                return Content("No docs available");
            }

            var m = new MarkdownSharp.Markdown();
            var t = m.Transform(System.IO.File.ReadAllText(file.FullName));

            return Content(t);
        }
    }
}