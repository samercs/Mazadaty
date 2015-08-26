using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.Admin.Models.Trophies;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("trophies"), RoleAuthorize(Role.Administrator)]
    public class TrophiesController : ApplicationController
    {
        private readonly TrophyService _trophyService;

        public TrophiesController(IAppServices appService) : base(appService)
        {
            _trophyService = new TrophyService(DataContextFactory);   
        } 

        public async Task<ActionResult> Index()
        {
            var model = await _trophyService.GetAll();
            return View(model);
        }

        [Route("edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            var trophy = await _trophyService.GetTrophy(id);
            if (trophy == null)
            {
                return HttpNotFound();
            }

            var viewModel = new EditViewModel
            {
                Trophy = trophy
            };

            return View(viewModel);
        }
        
        [Route("edit/{id}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EditViewModel model, LocalizedContent[] name, LocalizedContent[] description)
        {
            var trophy = await _trophyService.GetTrophy(id);
            if (trophy == null)
            {
                return HttpNotFound();
            }

            trophy.Name = name.Serialize();
            trophy.Description = description.Serialize();
            trophy.IconUrl = model.Trophy.IconUrl;
            trophy.XpAward = model.Trophy.XpAward;

            await _trophyService.Update(trophy);

            SetStatusMessage("Trophy successfully updated.");

            return RedirectToAction("Index");
        }
    }
}