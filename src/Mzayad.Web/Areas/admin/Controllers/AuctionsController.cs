using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Auction;
using Mzayad.Web.Areas.admin.Models.Products;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using IndexViewModel = Mzayad.Web.Areas.admin.Models.Auction.IndexViewModel;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("auctions")]
    public class AuctionsController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly AuctionServices _auctionServices;
        public AuctionsController(IControllerServices controllerServices) : base(controllerServices)
        {
            _productService=new ProductService(DataContextFactory);
            _auctionServices=new AuctionServices(DataContextFactory);
        }

        [Route("selectproduct")]
        public async Task<ActionResult> SelectProduct(string search="")
        {
            var products = await _productService.GetProductsWithoutCategory("en", search);
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }

            var model = new Mzayad.Web.Areas.admin.Models.Products.IndexViewModel()
            {
                Search = "",
                Products = products
            };


            return View(model);
        }

        public async Task<ActionResult> Create(int ProductId)
        {
            var product =await _productService.GetProduct(ProductId);
            if (product == null)
            {
                SetStatusMessage("Sorry this product not found.",StatusMessageType.Warning);
                return RedirectToAction("SelectProduct");
            }
            var model = await new CreateModelView().Hydrate(_productService, ProductId);
            model.Auction.RetailPrice = product.RetailPrice;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateModelView model, bool cbBuyNowEnabled)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Auction.StartUtc= model.Auction.StartUtc.AddHours(-3);
            model.Auction.BuyNowEnabled = cbBuyNowEnabled;
            if (!cbBuyNowEnabled)
            {
                model.Auction.BuyNowPrice = null;
                model.Auction.BuyNowQuantity = null;
                
            }
           
            
            await _auctionServices.Add(model.Auction);
            SetStatusMessage("Auction has been added successfully.");
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Index(string search="")
        {
            var model = new IndexViewModel()
            {
                Auctions=await _auctionServices.GetAuctions(search),
                Search = search
            };
            foreach (var auction in model.Auctions)
            {
                auction.Product.Localize("en", i => i.Name);
            }

            return View(model);
        }

        public async Task<JsonResult> GetAuctions([DataSourceRequest] DataSourceRequest request, string search=null)
        {
            var result = await _auctionServices.GetAuctions(search);
            foreach (var auction in result)
            {
                auction.Product.Localize("en", i => i.Name);
            }
            return Json(result.ToDataSourceResult(request));
        }

        public async Task<ExcelResult> DownloadExcel(string search = "")
        {
            var auctions = await _auctionServices.GetAuctions(search);
            foreach (var auction in auctions)
            {
                auction.Product.Localize("en", i => i.Name);
            }
            var results = auctions.Select(i => new
            {
                i.AuctionId,
                i.Product.Name,
                StartUtc=i.StartUtc.ToString("yyyy/MM/dd HH:mm"),
                Status=i.Status.Description(),
                i.RetailPrice,
                i.BidIncrement,
                i.Duration,
                i.MaximumBid,
                i.BuyNowEnabled,
                i.BuyNowPrice,
                i.BuyNowQuantity,
                Added = i.CreatedUtc.ToString("yyyy/MM/dd HH:mm")
                
            });

            return Excel(results, "auctions");
        }
	}
}