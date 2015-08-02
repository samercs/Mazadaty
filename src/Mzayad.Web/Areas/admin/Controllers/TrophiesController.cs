using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    public class TrophiesController : ApplicationController
    {
        private readonly TrophyService _trophyService;

        public TrophiesController(IAppServices appService) : base(appService)
        {
            _trophyService = new TrophyService(DataContextFactory);   
        } 
        // GET: Admin/Trophies
        public async Task<ActionResult> Index(string languageCode="en")
        {
            var model = await _trophyService.GetAll(languageCode);
            return View(model);
        }
    }
}