using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Mazadaty.Services;
using Mazadaty.Web.Core.Services;

namespace Mazadaty.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/banners")]
    public class BannersController : ApplicationApiController
    {
        private readonly BannerService _bannerService;

        public BannersController(IAppServices appServices) : base(appServices)
        {
            _bannerService = new BannerService(DataContextFactory);
        }

        [Route("")]
        public async Task<IHttpActionResult> GetBanners()
        {
            var banners = await _bannerService.GetAll();
            return Ok(banners.Select(i => new
            {
                i.BannerId,
                i.ImgMdUrl,
                i.ImgLgUrl,
                i.Url
            }));
        }
    }
}
