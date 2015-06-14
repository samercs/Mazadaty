using Mzayad.Models;
using Mzayad.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    public class LevelsController : Controller
    {
        // GET: Admin/Levels
        public ActionResult Index()
        {
            var levels = new List<Level>();
            var lvlService = new LevelService();
            for (var x = 0; x < 101; x++)
            {
                levels.Add(lvlService.GetLevel(x));
            }
            return View(levels);
        }
    }
}