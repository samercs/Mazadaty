using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Extensions;

namespace Mzayad.Web.Areas.admin.Models.Auctions
{
    public class AddEditViewModel
    {
        public IEnumerable<SelectListItem> ProductList { get; set; }
        public Mzayad.Models.Auction Auction { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }

        public async Task<AddEditViewModel> Hydrate(ProductService productService, int productId)
        {
            var product = await productService.GetProduct(productId);
            return await Hydrate(productService, product);
        }

        public async Task<AddEditViewModel> Hydrate(ProductService productService, Product product)
        {
            var minte = DateTime.Now.Minute >= 30 ? 30 : 0;
            var dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, minte, 0);
            
            Auction = new Mzayad.Models.Auction()
            {
                ProductId = product.ProductId,
                StartUtc = dateNow,
                Status = AuctionStatus.Hidden,
                RetailPrice = product.RetailPrice,
                BidIncrement = 1,
                Duration = 15,
                BuyNowEnabled = false,
                BuyNowPrice = product.RetailPrice,
                BuyNowQuantity = 0
            };

            ProductList = await GetProductList(productService, product);
            StatusList = GetStatusList(AuctionStatus.Hidden);

            return this;
        }

        private static async Task<IEnumerable<SelectListItem>>  GetProductList(ProductService productService, Product product)
        {
            var products = await productService.GetProducts("en");
            products = products.OrderBy(i => i.Name);

            return products.Select(item => new SelectListItem()
            {
                Text = item.Name,
                Value = item.ProductId.ToString(),
                Selected = item.ProductId == product.ProductId
            });
        }

        private static List<SelectListItem> GetStatusList(AuctionStatus auctionStatus)
        {
            return Enum.GetValues(typeof(AuctionStatus)).Cast<AuctionStatus>().Select(v => new SelectListItem
            {
                Text = string.Format("<strong>{0}</strong> &mdash; {1}", v, v.Description()),
                Value = ((int)v).ToString(),
                Selected = (v == auctionStatus)

            }).ToList();
        }

        public async Task<AddEditViewModel> Hydrate(ProductService productService, Mzayad.Models.Auction auction)
        {
            Auction = auction;
            Auction.StartUtc = auction.StartUtc.AddHours(3);
            
            ProductList = await GetProductList(productService, auction.Product);
            StatusList = GetStatusList(Auction.Status);

            return this;
        } 
    }
}