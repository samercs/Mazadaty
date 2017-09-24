using Mazadaty.Models;
using Mazadaty.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.Admin.Controllers
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
