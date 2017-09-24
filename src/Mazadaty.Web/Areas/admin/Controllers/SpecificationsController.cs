using Mazadaty.Models;
using Mazadaty.Services;
using Mazadaty.Web.Areas.admin.Models.Specification;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.admin.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    public class SpecificationsController : ApplicationController
    {
        private readonly SpecificationService _specificationService;

        public SpecificationsController(IAppServices appServices)
            : base(appServices)
        {
            _specificationService = new SpecificationService(DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var model = await _specificationService.GetAll("en");

            return View(model);
        }

        public ActionResult Add()
        {
            var model = new AddViewModel().Hydrate();
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Specification = new Specification { Name = name.Serialize() };

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var specification = await _specificationService.Add(model.Specification);
            specification.Localize("en", i => i.Name);
            SetStatusMessage($"Specification <strong>{specification.Name}</strong> successfully added.");
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            var specification = await _specificationService.GetById(id);
            if (specification == null)
            {
                return HttpNotFound();
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
            SetStatusMessage($"Specification <strong>{name}</strong> successfully deleted.");
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

            SetStatusMessage($"Specification <strong>{specificationName}</strong> successfully updated.");

            return RedirectToAction("Index");
        }
    }
}
