using Mzayad.Services;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [LanguageRoutePrefix("products")]
    public class ProductsController : ApplicationController
    {
        private readonly ProductService _productService;

        public ProductsController(IAppServices appServices) : base(appServices)
        {
            _productService = new ProductService(DataContextFactory);
        }


        [Route("{productId:int}")]
        public async Task<ActionResult> Index(int productId)
        {
            var product = await _productService.GetProduct(productId);
            product.Localize(Language, i => i.Name, i => i.Description);

            if (product.SponsorId.HasValue)
            {
                product.Sponsor.Localize(Language, i => i.Name);
            }

            if (product.ProductSpecifications.Any())
            {
                foreach (var specification in product.ProductSpecifications)
                {
                    specification.Specification.Localize(Language, i => i.Name);
                    specification.Localize(Language, i => i.Value);
                }
            }

            return View(product);
        }
    }
}