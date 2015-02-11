using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class DocsController : ApplicationController
    {
        public DocsController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

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