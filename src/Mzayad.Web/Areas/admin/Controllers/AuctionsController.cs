using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Auction;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Web.Areas.admin.Models.Auctions;

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

        [Route("select-product")]
        public async Task<ActionResult> SelectProduct(string search="")
        {
            var products = await _productService.GetProductsWithoutCategory("en", search);
            foreach (var product in products)
            {
                product.Description = StringFormatter.StripHtmlTags(product.Description);
            }

            var model = new Models.Products.IndexViewModel()
            {
                Search = "",
                Products = products
            };


            return View(model);
        }

        public async Task<ActionResult> Create(int productId)
        {
            var product =await _productService.GetProduct(productId);
            if (product == null)
            {
                SetStatusMessage("Sorry this product not found.",StatusMessageType.Warning);
                return RedirectToAction("SelectProduct");
            }
            var model = await new AddEditViewModel().Hydrate(_productService, product);
            model.Auction.RetailPrice = product.RetailPrice;
            model.Auction.CreatedByUserId = AuthService.CurrentUserId();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int productId, AddEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await model.Hydrate(_productService, productId));
            }
            
            model.Auction.StartUtc = model.Auction.StartUtc.AddHours(-3);       
            
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
                StartUtc = i.StartUtc.ToString("yyyy/MM/dd HH:mm"),
                Status = i.Status.Description(),
                i.RetailPrice,
                i.ReservePrice,
                i.BidIncrement,
                i.Duration,
                i.MaximumBid,
                i.BuyNowEnabled,
                i.BuyNowPrice,
                i.BuyNowQuantity,
                CreatedUtc = i.CreatedUtc.ToString("yyyy/MM/dd HH:mm")
                
            });

            return Excel(results, "auctions");
        }

        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {
            var auction = await _auctionServices.GetAuction(id);
            if (auction == null)
            {
                SetStatusMessage("Sorry this auction not found",StatusMessageType.Warning);
                return RedirectToAction("Index", "Auctions");
            }

            var model = await new AddEditViewModel().Hydrate(_productService, auction);
            return View(model);
        }

        [Route("edit/{id:int}")]
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AddEditViewModel model)
        {
            var auction = await _auctionServices.GetAuction(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            auction.BidIncrement = model.Auction.BidIncrement;
            auction.Duration = model.Auction.Duration;
            auction.MaximumBid = model.Auction.MaximumBid;
            auction.ProductId = model.Auction.ProductId;
            auction.RetailPrice = model.Auction.RetailPrice;
            auction.StartUtc = model.Auction.StartUtc.AddHours(-3);
            auction.Status = model.Auction.Status;
            auction.BuyNowEnabled = model.Auction.BuyNowEnabled;
            auction.BuyNowPrice = model.Auction.BuyNowPrice;
            auction.BuyNowQuantity = model.Auction.BuyNowQuantity;

            await _auctionServices.Update(auction);
            SetStatusMessage("Auction has been updated successfully.");
            return RedirectToAction("Index", "Auctions");     
        }

	}
}