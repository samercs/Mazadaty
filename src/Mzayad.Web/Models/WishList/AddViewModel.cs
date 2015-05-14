using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Services;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Models.WishList
{
    public class AddViewModel
    {
        public Mzayad.Models.WishList WishList { get; set; }

        public IEnumerable<string> ProductList { get; set; }

        public async Task<AddViewModel> Hydrate(IAuthService authService,ProductService productService)
        {
            if (WishList == null)
            {
                WishList = new Mzayad.Models.WishList()
                {
                    UserId   = authService.CurrentUserId()
                };
            }

            var allEnProduct = await productService.GetProducts("en");

            var allArProduct = await productService.GetProducts("ar");

            ProductList = allEnProduct.Concat(allArProduct).Select(i => i.Name);

            return this;
        }
    }
}
