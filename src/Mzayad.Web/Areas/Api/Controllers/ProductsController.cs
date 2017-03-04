using System.Linq;
using Mzayad.Services;
using Mzayad.Web.Areas.Api.Models.Products;
using Mzayad.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Http;
using Mzayad.Models;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApplicationApiController
    {
        private readonly ProductService _productService;
        private readonly AuctionService _auctionService;

        public ProductsController(IAppServices appServices) : base(appServices)
        {
            _productService = new ProductService(DataContextFactory);
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
        }

        [Route("{productId:int}")]
        public async Task<IHttpActionResult> GetProduct(int productId)
        {
            var product = await _productService.GetProduct(productId);
            product.Localize(Language, LocalizationDepth.OneLevel);
            if (product.ProductSpecifications != null)
            {
                product.ProductSpecifications = product.ProductSpecifications
                    .Localize<ProductSpecification>(Language, LocalizationDepth.OneLevel)
                    .ToList();
            }

            return Ok(ProductModel.Create(product));
        }

        [Route("buy-now")]
        public async Task<IHttpActionResult> GetBuyNowProducts()
        {
            var buyNowAuctions = await _auctionService.GetBuyNowAuctions(Language);
            return Ok(buyNowAuctions.Select(ProductModel.Create));
        }
    }
}
