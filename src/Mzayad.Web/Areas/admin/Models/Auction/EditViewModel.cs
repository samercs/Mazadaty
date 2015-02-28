using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Extensions;

namespace Mzayad.Web.Areas.admin.Models.Auction
{
    public class EditViewModel
    {
        public Mzayad.Models.Auction Auction { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public async Task<EditViewModel> Hydrate(ProductService productService, Mzayad.Models.Auction auction)
        {
            
            Auction = auction;
            Auction.StartUtc = auction.StartUtc.AddHours(3);
            var products = await productService.GetProducts("en");
            products = products.OrderBy(i => i.Name);
            ProductList = new List<SelectListItem>();
            foreach (var product in products)
            {
                ProductList.Add(new SelectListItem()
                {
                    Text = product.Name,
                    Value = product.ProductId.ToString(),
                    Selected = product.ProductId == auction.ProductId
                });
            }

            StatusList = Enum.GetValues(typeof(AuctionStatus)).Cast<AuctionStatus>().Select(v => new SelectListItem
            {
                Text = v.ToString() + " — " + v.Description(),
                Value = ((int)v).ToString(),
                Selected = (v == Auction.Status)

            }).ToList();



            return this;

        }
    }
}