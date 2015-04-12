using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Sponsors;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class SponsorsController : ApplicationController
    {
        private readonly SponsorService _sponsorService;

        public SponsorsController(IAppServices appServices) : base(appServices)
        {
            _sponsorService=new SponsorService(appServices.DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var model = await _sponsorService.GetAll();
            model=model.Localize("en", i => i.Name);
            return View(model);
        }

        public ActionResult Add()
        {
            var model = new AddViewModel().Hydrate();
            return View(model);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Sponsor.Name = name.Serialize();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newSponsor= await _sponsorService.Add(model.Sponsor);

            var sponsorName = newSponsor.Localize("en", i => i.Name).Name;
            SetStatusMessage(string.Format("Sponsor <strong>{0}</strong> has been added successfully.",sponsorName));
            return RedirectToAction("Index");

        }

        public async Task<ActionResult> Edit(int id)
        {
            var sponsor = await _sponsorService.GetById(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }

            var model = new AddViewModel();
            model.Sponsor = sponsor;
            return View(model);

        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AddViewModel model, LocalizedContent[] name)
        {
            var sponsor = await _sponsorService.GetById(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }

            sponsor.Name = name.Serialize();
            var newSponsor= await _sponsorService.Save(sponsor);
            var newSponsorName = newSponsor.Localize("en", i => i.Name).Name;
            SetStatusMessage(string.Format("sponsor <strong>{0}</strong> has been saved successfully.",newSponsorName));
            return RedirectToAction("Index");

        }

        public async Task<ActionResult> Delete(int id)
        {
            var sponsor = await _sponsorService.GetById(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }

            var sponsorName = sponsor.Localize("en", i => i.Name).Name;
            var confirmation = string.Format("Are you sure you want to permanently delete <strong>{0}</strong>?", sponsorName);

            return DeleteConfirmation("Delete Confirmation", confirmation);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, FormCollection formCollection)
        {
            var sponsor = await _sponsorService.GetById(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }

            await _sponsorService.Delete(sponsor);
            
            SetStatusMessage("Sponsor has been deleted successfully.");
            return RedirectToAction("Index");
        }
    }
}