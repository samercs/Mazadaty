using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Specification;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    public class SpecificationsController : ApplicationController
    {
        private readonly SpecificationService _specificationService;

        public SpecificationsController(IControllerServices controllerServices)
            : base(controllerServices)
        {
            _specificationService = new SpecificationService(DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var model = await _specificationService.GetAll("en");

            return View(model);
        }

        public async Task<ActionResult> Add()
        {
            var model = await new AddViewModel().Hydrate();
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Specification.Name = name.Serialize();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var specification = await _specificationService.Add(model.Specification);
            specification.Localize("en", i => i.Name);
            SetStatusMessage(string.Format("Specification <strong>{0}</strong> successfully added.", specification.Name));
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            var specification = await _specificationService.GetById(id);
            if (specification == null)
            {
                SetStatusMessage("sory specification not found.", StatusMessageType.Error);
                return RedirectToAction("Index");
            }
            specification = specification.Localize("en", i => i.Name);
            return View(specification);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Specification model)
        {
            var specification = await _specificationService.GetById(model.SpecificationId);
            if (specification == null)
            {
                return HttpNotFound();
            }

            var name = specification.Localize("en", i => i.Name).Name;
            await _specificationService.Delete(specification);
            SetStatusMessage(string.Format("Specification <strong>{0}</strong> successfully deleted.", name));
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            var specification = await _specificationService.GetById(id);
            if (specification == null)
            {
                return HttpNotFound();
            }

            var model = new AddViewModel
            {
                Specification = specification
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AddViewModel model, LocalizedContent[] name)
        {
            var specification = await _specificationService.GetById(id);
            if (specification == null)
            {
                return HttpNotFound();
            }

            if (!TryUpdateModel(specification, "Specification"))
            {
                return View(model);
            }

            specification.Name = name.Serialize();

            await _specificationService.Update(specification);

            var specificationName = specification.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Specification <strong>{0}</strong> successfully updated.", specificationName));

            return RedirectToAction("Index");
        }
    }
}