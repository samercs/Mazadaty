using Mzayad.Models;
using Mzayad.Services;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Models.Products
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