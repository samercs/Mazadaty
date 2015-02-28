using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Auction;
using Mzayad.Web.Areas.admin.Models.Products;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;

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

            var model = new IndexViewModel()
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
        public async Task<ActionResult> Create(CreateModelView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Auction.StartUtc= model.Auction.StartUtc.AddHours(-3);
            if (!model.Auction.BuyNowEnabled.HasValue )
            {
                model.Auction.BuyNowPrice = null;
                model.Auction.BuyNowQuantity = null;
            }
            else if(!model.Auction.BuyNowEnabled.Value)
            {
                model.Auction.BuyNowPrice = null;
                model.Auction.BuyNowQuantity = null;
            }
            await _auctionServices.Add(model.Auction);
            SetStatusMessage("Auction has been added successfully.");
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}