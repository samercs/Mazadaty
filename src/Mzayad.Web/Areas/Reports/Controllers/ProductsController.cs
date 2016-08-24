using Mzayad.Services;
using Mzayad.Web.Areas.Reports.Models.Products;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using OrangeJetpack.Base.Web;

namespace Mzayad.Web.Areas.Reports.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    [RouteArea("reports"), RoutePrefix("products")]
    public class ProductsController : ApplicationController
    {
        private readonly ProductService _productService;

        public ProductsController(IAppServices appServices) : base(appServices)
        {
            _productService = new ProductService(DataContextFactory);
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            var model = new IndexViewModel
            {
                DateRange = new DateRangeModel
                {
                    StartDate = DateTime.Now.AddDays(-7),
                    EndDate = DateTime.Now
                }
            };

            model.Products = await _productService.GetProductsByDate(model.DateRange.StartDate, model.DateRange.EndDate);
            return View(model);
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult> Index(IndexViewModel model)
        {
            if (model.DateRange.StartDate > model.DateRange.EndDate)
            {
                SetStatusMessage("End date is greater than start date, please pick another dates and try again.", StatusMessageType.Warning);
                return View(model);
            }

            model.Products = await _productService.GetProductsByDate(model.DateRange.StartDate, model.DateRange.EndDate);
            return View(model);
        }
    }
}