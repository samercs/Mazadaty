using Mazadaty.Services;
using Mazadaty.Web.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mazadaty.Web.Models.WishList
{
    public class AddViewModel
    {
        public Mazadaty.Models.WishList WishList { get; set; }
        public IEnumerable<string> ProductList { get; set; }

        public async Task<AddViewModel> Hydrate(IAuthService authService, ProductService productService)
        {
            if (WishList == null)
            {
                WishList = new Mazadaty.Models.WishList()
                {
                    UserId = authService.CurrentUserId()
                };
            }

            ProductList = await productService.GetProductNames();

            return this;
        }
    }
}
