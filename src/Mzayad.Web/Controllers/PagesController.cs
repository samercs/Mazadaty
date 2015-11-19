using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [LanguageRoutePrefix("pages")]
    public class PagesController : ApplicationController
    {

        public PagesController(IAppServices appServices) : base(appServices)
        {
        }

        [Route("{slug}")]
        public async Task<ActionResult> Index(string slug)
        {
            var cacheKey = string.Format(CacheKeys.SitePage, Language, slug);
            var page = await CacheService.TryGet(cacheKey, async () => await CmsClient.GetPageBySlug(slug, Language));
            if (page == null)
            {
                return HttpNotFound();
            }

            return View(page);
        }
    }
}