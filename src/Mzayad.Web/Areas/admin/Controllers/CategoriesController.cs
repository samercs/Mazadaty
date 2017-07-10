using Mzayad.Models;
using Mzayad.Web.Areas.admin.Models.Categories;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("categories"), RoleAuthorize(Role.Administrator)]
    public class CategoriesController : ApplicationController
    {
        private readonly CategoryService _categoryService;
        
        public CategoriesController(IAppServices appServices) : base(appServices)
        {
            _categoryService = new CategoryService(appServices.DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var categories = await _categoryService.GetCategoriesAsHierarchy();

            return View(categories);
        }

        [Route("add")]
        public async Task<ActionResult> Add()
        {
            var model = await new AddViewModel().Hydrate(_categoryService);

            return View(model);
        }


        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {
            model.Category.Name = name.Serialize();
            ModelState["Category.Name"].Errors.Clear();

            if (!ModelState.IsValid)
            {
                await model.Hydrate(_categoryService);
                return View(model);
            }

            var category = await _categoryService.AddCategory(model.Category);          
            var categoryName = category.Localize("en", i => i.Name).Name;

            SetStatusMessage($"Category {categoryName} successfully added.");

            return RedirectToAction("Index");
        }


        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategory(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var viewModel = await new EditViewModel().Hydrate(category, _categoryService);

            return View(viewModel);
        }

        [Route("edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EditViewModel model, LocalizedContent[] name)
        {
            var category = await _categoryService.GetCategory(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            if (!TryUpdateModel(category, "Category"))
            {
                var viewModel = await new EditViewModel().Hydrate(category, _categoryService);

                return View(viewModel);
            }

            category.Name = name.Serialize();

            await _categoryService.UpdateCategory(category);
            
            var categoryName = category.Localize("en", i => i.Name).Name;

            SetStatusMessage(string.Format("Category {0} successfully updated.", categoryName));

            return RedirectToAction("Index");
        }


        [Route("delete/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategory(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            _categoryService.Delete(category);
            category.Localize("en", i => i.Name);
            SetStatusMessage(string.Format("Category {0} successfully deleted.", category.Name));

            return RedirectToAction("Index");
        }
	}
}