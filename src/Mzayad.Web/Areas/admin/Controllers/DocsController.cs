using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    public class DocsController : ApplicationController
    {
        public DocsController(IAppServices appServices) : base(appServices)
        {
        }

        private static IReadOnlyCollection<FileInfo> GetMarkdownFiles()
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            var docs = Path.Combine(home, @"site\wwwdocs");
            var dir = new DirectoryInfo(docs);

            return dir.GetFiles("*.md").OrderByDescending(i => i.Name).ToList();
        }

        public ActionResult Index()
        {
            return View(GetMarkdownFiles());
        }

        public ActionResult Item(string file)
        {
            var fileInfo = GetMarkdownFiles().SingleOrDefault(i => i.Name == file);
            if (fileInfo == null)
            {
                return HttpNotFound();
            }

            return View(fileInfo);
        }
    }
}