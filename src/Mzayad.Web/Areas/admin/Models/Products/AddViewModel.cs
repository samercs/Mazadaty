using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Mzayad.Models;
using Mzayad.Services;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Models.Products
{
    public class AddViewModel
    {
        public Product Product { get; set; }


        public async Task<AddViewModel> Hydrate(ProductService productService)
        {
            if (Product == null)
            {
                Product = new Product
                {
                    Name = LocalizedContent.Init(),
                    Description = LocalizedContent.Init()
                };
            }

            return this;
        }
    }
}