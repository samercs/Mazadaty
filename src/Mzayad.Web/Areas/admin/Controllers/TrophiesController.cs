using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;

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

        public async Task<ActionResult> Edit(int id)
        {
            var trophy = await _trophyService.GetTrophy(id);
            if (trophy == null)
            {
                return HttpNotFound();
            }
            return View(trophy);
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Trophy model, LocalizedContent[] name, LocalizedContent[] description)
        {
            var trophy = await _trophyService.GetTrophy(id);
            if (trophy == null)
            {
                return HttpNotFound();
            }

            //if (!TryUpdateModel(trophy, "Trophy"))
            //{
            //    return View(trophy);
            //}

            trophy.Name = name.Serialize();
            trophy.Description = description.Serialize();

            await _trophyService.Update(trophy);

            //var trophyName = trophy.Localize("en", i => i.Name).Name;

            //SetStatusMessage($"Trophy {trophyName} successfully updated.");

            return RedirectToAction("Index");
        }
    }
}