using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Configuration;
using Mazadaty.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Models;
using Mazadaty.Services;

namespace Mazadaty.Web.Controllers
{
    [LanguageRoutePrefix("pages")]
    public class PagesController : ApplicationController
    {
        private readonly PageService _pageService;
        public PagesController(IAppServices appServices) : base(appServices)
        {
            _pageService = new PageService(DataContextFactory);
        }

        [Route("{slug}")]
        public async Task<ActionResult> Index(string slug)
        {
            var cacheKey = string.Format(CacheKeys.SitePage, Language, slug);
            var page = await CacheService.TryGetAsync(cacheKey, () => _pageService.GetByTag(slug+"-"+Language));
            if (page == null)
            {
                return HttpNotFound();
            }

            return View(page);
        }
    }
}
