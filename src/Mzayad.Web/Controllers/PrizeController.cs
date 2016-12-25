using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Web.Models.Prize;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/prize")]
    public class PrizeController : ApplicationController
    {
        private readonly PrizeService _prizeService;
        public PrizeController(IAppServices appServices) : base(appServices)
        {
            _prizeService = new PrizeService(DataContextFactory);
        }


        [Route("")]
        public async Task<ActionResult> Index()
        {
            var prizes = await _prizeService.GetAvaliablePrize();
            var data = prizes.Select(i => new { Title = i.Title, Weight = i.Weight });
            var model = new IndexViewModel
            {
                PrizesJson =
                    JsonConvert.SerializeObject(data, Formatting.Indented,
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            };
            return View(model);
        }


    }
}