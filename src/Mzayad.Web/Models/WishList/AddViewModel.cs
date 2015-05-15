using Mzayad.Services;
using Mzayad.Web.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            ProductList = await productService.GetProductNames();

            return this;
        }
    }
}
