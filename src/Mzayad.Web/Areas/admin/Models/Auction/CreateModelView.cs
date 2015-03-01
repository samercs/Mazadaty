using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Extensions;

namespace Mzayad.Web.Areas.admin.Models.Auction
{
    public class CreateModelView
    {
        
        public List<SelectListItem> ProductList { get; set; }
        public Mzayad.Models.Auction Auction { get; set; }
        public List<SelectListItem> StatusList { get; set; } 

        public async Task<CreateModelView> Hydrate(ProductService productService, int productId)
        {
            var minte = DateTime.Now.Minute >= 30 ? 30 : 0;
            var dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, minte, 0);
            
            Auction = new Mzayad.Models.Auction()
            {
                ProductId = productId,
                StartUtc = dateNow,
                Status = AuctionStatus.Hidden,
                Duration = 15,
                BuyNowEnabled = true
            };

            var products = await productService.GetProducts("en");
            products=products.OrderBy(i => i.Name);
            ProductList=new List<SelectListItem>();
            foreach (var product in products)
            {
                ProductList.Add(new SelectListItem()
                {
                    Text = product.Name,
                    Value = product.ProductId.ToString(),
                    Selected = product.ProductId==productId
                });
            }

            StatusList= Enum.GetValues(typeof(AuctionStatus)).Cast<AuctionStatus>().Select(v => new SelectListItem
            {
                Text = string.Format("<strong>{0}</strong> &mdash; {1}", v, v.Description()),
                Value = ((int)v).ToString(),
                Selected = (v==AuctionStatus.Hidden)
                
            }).ToList();

            

            return this;

        }
    }
}