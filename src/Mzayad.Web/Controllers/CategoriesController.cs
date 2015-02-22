using Mzayad.Services;
using Mzayad.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/categories")]
    public class CategoriesController : ApplicationController
    {
        private readonly CategoryService _categoryService;
        
        public CategoriesController(IControllerServices controllerServices) : base(controllerServices)
        {
            _categoryService = new CategoryService(controllerServices.DataContextFactory);
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