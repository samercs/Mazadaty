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

        public static ProductModel Create(Auction auction)
        {
            return new ProductModel
            {
                ProductId = auction.ProductId,
                Name = auction.Title,
                Price = auction.BuyNowPrice ?? decimal.Zero,
                SponsorName = auction.Product.SponsorId.HasValue ? auction.Product.Sponsor.Name : null,
                MainImageUrl = auction.Product.MainImage().ImageMdUrl
            };
        }
    }
}