using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Models.Auctions
{
    public class AddEditViewModel
    {
        public IEnumerable<SelectListItem> ProductList { get; set; }
        public Mzayad.Models.Auction Auction { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public IEnumerable<Bid> Bids { get; set; }
        public Product Product { get; set; }
        public ActionType ActionType { get; set; }

        public IEnumerable<SelectListItem> GccCountryList => new[] {
                new SelectListItem
                {
                    Text = Resources.Global.Kuwait,
                    Value = "KW",
                    Selected = Auction?.CountryList?.Contains("KW") ?? false
                },
                new SelectListItem
                {
                    Text = Resources.Global.SaudiArabia,
                    Value = "SA",
                    Selected = Auction?.CountryList?.Contains("SA") ?? false
                },
                new SelectListItem
                {
                    Text = Resources.Global.UAE,
                    Value = "AE",
                    Selected = Auction?.CountryList?.Contains("AE") ?? false
                },
                new SelectListItem
                {
                    Text = Resources.Global.Bahrain,
                    Value = "BH",
                    Selected = Auction?.CountryList?.Contains("BH") ?? false
                },
                new SelectListItem
                {
                    Text = Resources.Global.Qatar,
                    Value = "QA",
                    Selected = Auction?.CountryList?.Contains("QA") ?? false
                },
                new SelectListItem
                {
                    Text = Resources.Global.Oman,
                    Value = "OM",
                    Selected = Auction?.CountryList?.Contains("OM") ?? false
                }
            };
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
                BuyNowQuantity = 0,
                CountryList = ""
            };

            ProductList = await GetProductList(productService, product);
            StatusList = GetStatusList(AuctionStatus.Hidden);
            Product = product;
            ActionType = ActionType.Add;


            return this;
        }

        private static async Task<IEnumerable<SelectListItem>> GetProductList(ProductService productService, Product product)
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
            Auction.StartUtc = auction.StartUtc.AddHours(3); // UTC > AST

            ProductList = await GetProductList(productService, auction.Product);
            StatusList = GetStatusList(Auction.Status);
            Product = auction.Product;
            ActionType = ActionType.Edit;
            return this;
        }
    }

    public enum ActionType
    {
        Add = 1,
        Edit = 2
    }
}