using Mazadaty.Services;
using Mazadaty.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Web.Core.Attributes;

namespace Mazadaty.Web.Controllers
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
