using Mzayad.Services;
using Mzayad.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Web.Core.Attributes;

namespace Mzayad.Web.Controllers
{
    [LanguageRoutePrefix("categories")]
    public class CategoriesController : ApplicationController
    {
        private readonly CategoryService _categoryService;
        
        public CategoriesController(IAppServices appServices) : base(appServices)
        {
            _categoryService = new CategoryService(appServices.DataContextFactory);
        }

        [Route("{slug}")]
        public async Task<ActionResult> Index(string slug)
        {
            var category = await _categoryService.GetCategoryBySlug(slug);
            if (category == null)
            {
                return HttpNotFound();
            }

            return Content(category.Name);
        }
    }
}