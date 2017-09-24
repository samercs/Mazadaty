using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Models;
using Mazadaty.Services;
using Mazadaty.Web.Areas.Admin.Models.Trophies;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using OrangeJetpack.Localization;

namespace Mazadaty.Web.Areas.Admin.Controllers
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
