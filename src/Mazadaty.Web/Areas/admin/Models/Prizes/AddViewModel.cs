using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.Admin.Models.Prizes
{
    public class AddViewModel
    {
        public Prize Prize { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public IEnumerable<SelectListItem> PrizeTypeList { get; set; }

        public async Task<AddViewModel> Hydrate(ProductService productService)
        {
            PrizeTypeList = GetPrizeTypeList(); 
            ProductList = await GetProductList(productService, Prize.Product);
            StatusList = GetStatusList();
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
                Selected = item.ProductId == product?.ProductId
            });
        }

        private List<SelectListItem> GetStatusList()
        {
            return Enum.GetValues(typeof(PrizeStatus)).Cast<PrizeStatus>().Select(v => new SelectListItem
            {
                Text = $"<strong>{v}</strong> &mdash; {v.Description()}",
                Value = ((int)v).ToString(),
                Selected = (Prize.Status == v)

            }).ToList();
        }

        private List<SelectListItem> GetPrizeTypeList()
        {
            return Enum.GetValues(typeof(PrizeType)).Cast<PrizeType>().Select(v => new SelectListItem
            {
                Text = $"<strong>{v}</strong>",
                Value = ((int)v).ToString(),
                Selected = (Prize.PrizeType == v)

            }).ToList();
        }
    }
}
