using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Web.Areas.admin.Models.Categories;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("Admin"), RoutePrefix("categories")]
    public class CategoriesController : ApplicationController
    {
        //
        // GET: /admin/Categories/
        public CategoriesController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public async Task<ActionResult> Index()
        {
            var categories = await _CategoryService.GetCategoriesAsHierarchyAsync();

            return View(categories);
        }

        [Route("add")]
        public async Task<ActionResult> Add()
        {
            var model = await new AddViewModel().Hydrate(_CategoryService);

            return View(model);
        }


        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Category.Name = name.Serialize();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Category category = null;
            
            category = await _CategoryService.AddCategory(model.Category);
             
            var categoryName = category.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Category {0} successfully added.", categoryName));

            return RedirectToAction("Index");
        }
	}
}