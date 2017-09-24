using Mazadaty.Models;
using Mazadaty.Services;
using OrangeJetpack.Localization;

namespace Mazadaty.Web.Areas.admin.Models.Products
{
    public class AddViewModel
    {
        public Product Product { get; set; }
        public bool GoToAuction { get; set; }

        public AddViewModel Hydrate(ProductService productService)
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
