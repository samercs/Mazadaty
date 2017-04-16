using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mzayad.Web.Areas.Api.Models.Ads;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/ads")]
    public class AdsController : ApplicationApiController
    {
        public AdsController(IAppServices appServices) : base(appServices)
        {
        }

        [Route("mobile/random")]
        public IHttpActionResult GetRandomAds()
        {
            var advModel = new AdsViewModel()
            {
                ImageUrl = "https://az723232.vo.msecnd.net/assets/zeedli-logo-web-636279368563125419.svg",
                MobileAdId = 1,
                Weight = 1
            };
            return Ok(advModel);
        }
    }
}
