using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class SpecificationsController : ApplicationController
    {

        private readonly SpecificationService _specificationService;

        public SpecificationsController(IControllerServices controllerServices) : base(controllerServices)
        {
            _specificationService=new SpecificationService(DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var model = await _specificationService.GetAll();
            return View(model);
        }

        public async Task<ActionResult> Add()
        {
            var model = new Specification()
            {
                Name = LocalizedContent.Init()
            };
            return View(model);
        }


        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Specification model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var specification = await _specificationService.Add(model);
            specification.Localize("en", i => i.Name);
            SetStatusMessage(string.Format("Specification {0} has been added successfully.",specification.Name));
            return RedirectToAction("Index");
        }
    }
}