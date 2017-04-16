using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Mzayad.Services;
using Mzayad.Web.Areas.Api.Models.Ads;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/ads")]
    public class AdsController : ApplicationApiController
    {
        private readonly SplashAdService _splashAdService;
        public AdsController(IAppServices appServices) : base(appServices)
        {
            _splashAdService = new SplashAdService(DataContextFactory);
        }

        [Route("mobile/random")]
        public async Task<IHttpActionResult> GetRandomAds()
        {
            var ad = await _splashAdService.GetRandom();

            var advModel = AdsViewModel.Create(ad);
            return Ok(advModel);
        }
    }
}
