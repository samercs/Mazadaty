using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Web.Areas.Admin.Models.Prizes;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.Admin.Controllers
{
    [RoutePrefix("prizes"), RouteArea("admin"), RoleAuthorize(Role.Administrator)]
    public class PrizesController : ApplicationController
    {
        private readonly PrizeService _prizeService;
        private readonly ProductService _productService;
        public PrizesController(IAppServices appServices) : base(appServices)
        {
            _prizeService = new PrizeService(DataContextFactory);
            _productService = new ProductService(DataContextFactory);
        }
        [Route("")]
        public async Task<ActionResult> Index(string search = "")
        {
            var model = new IndexViewModel
            {
                Prizes = await _prizeService.GetAll(Language, search)
            };

            return View(model);
        }

        public async Task<JsonResult> GetPrizes([DataSourceRequest] DataSourceRequest request, string search = null)
        {
            var result = await _prizeService.GetAll(Language, search);
            return Json(result.ToDataSourceResult(request));
        }

        [Route("add")]
        public async Task<ActionResult> Add()
        {
            var model = await new AddViewModel
            {
                Prize = new Prize
                {
                    Title = LocalizedContent.Init()
                }
            }.Hydrate(_productService);
            return View(model);
        }

        [Route("add"), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] title)
        {
            var prize = new Prize
            {
                PrizeType = model.Prize.PrizeType,
                Title = title.Serialize(),
                Limit = model.Prize.Limit,
                Weight = model.Prize.Weight,
                Status = model.Prize.Status,
                ProductId = model.Prize.PrizeType == PrizeType.Product ? model.Prize.ProductId : null,
                SubscriptionDays = model.Prize.PrizeType == PrizeType.Subscription ? model.Prize.SubscriptionDays : null
            };

            await _prizeService.Add(prize);
            SetStatusMessage("Prize has been added successfully.");
            return RedirectToAction("Index");
        }

        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {

            var prize = await _prizeService.GetById(id);
            if (prize == null)
            {
                return HttpNotFound();
            }
            var model = await new AddViewModel
            {
                Prize = prize
            }.Hydrate(_productService);
            return View(model);
        }

        [Route("edit/{id:int}"), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AddViewModel model, LocalizedContent[] title)
        {

            var prize = await _prizeService.GetById(id);
            if (prize == null)
            {
                return HttpNotFound();
            }
            prize.PrizeType = model.Prize.PrizeType;
            prize.Title = title.Serialize();
            prize.Limit = model.Prize.Limit;
            prize.Weight = model.Prize.Weight;
            prize.Status = model.Prize.Status;
            prize.ProductId = model.Prize.PrizeType == PrizeType.Product ? model.Prize.ProductId : null;
            prize.SubscriptionDays = model.Prize.PrizeType == PrizeType.Subscription
                ? model.Prize.SubscriptionDays
                : null;

            await _prizeService.Save(prize);
            SetStatusMessage("Prize has been saved successfully.");
            return RedirectToAction("Index");
        }



    }
}
