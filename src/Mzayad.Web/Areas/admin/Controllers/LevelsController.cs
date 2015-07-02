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
            var lvlService = new LevelService();
            for (var x = 1; x <= 100; x++)
            {
                levels.Add(lvlService.GetLevel(x));
            }
            return View(levels);
        }
    }
}