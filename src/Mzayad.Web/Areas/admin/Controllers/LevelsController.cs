using Mzayad.Models;
using Mzayad.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    public class LevelsController : Controller
    {
        public ActionResult Index()
        {
            var levels = new List<Level>();
            for (var x = 1; x <= 100; x++)
            {
                levels.Add(LevelService.GetLevel(x));
            }
            return View(levels);
        }
    }
}