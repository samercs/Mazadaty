using Mzayad.Models;

namespace Mzayad.Web.Areas.Api.Models.Products
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string SponsorName { get; set; }

        public string VideoUrl { get; set; }

        public string MainImageUrl { get; set; }

        public string Description { get; set; }

        public static ProductModel Create(Product product)
        {
            return new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.RetailPrice,
                SponsorName = product.SponsorId.HasValue ? product.Sponsor.Name : null,
                VideoUrl = product.VideoUrl,
                MainImageUrl = product.MainImage().ImageMdUrl,
                Description = product.Description
            };
        }
    }
}