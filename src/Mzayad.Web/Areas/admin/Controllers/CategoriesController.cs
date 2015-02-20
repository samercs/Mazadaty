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


        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {
            var category = await _CategoryService.GetCategory(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var viewModel = await new EditViewModel().Hydrate(category, _CategoryService);

            return View(viewModel);
        }

        [Route("edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EditViewModel model, LocalizedContent[] name)
        {
            var category = await _CategoryService.GetCategory(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            if (!TryUpdateModel(category, "Category"))
            {
                var viewModel = await new EditViewModel().Hydrate(category, _CategoryService);

                return View(viewModel);
            }

            category.Name = name.Serialize();

            await _CategoryService.UpdateCategory(category);
            
            var categoryName = category.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Category {0} successfully updated.", categoryName));

            return RedirectToAction("Index");
        }


        [Route("delete/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var category = await _CategoryService.GetCategory(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            _CategoryService.Delete(category);
            category.Localize("en", i => i.Name);
            SetStatusMessage(string.Format("Category {0} successfully deleted.", category.Name));

            return RedirectToAction("Index");
        }
	}
}