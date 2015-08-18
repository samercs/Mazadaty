using Mzayad.Models;

namespace Mzayad.Web.Models.Auctions
{
    public class AuctionImageViewModel
    {
        public string ImageSmUrl { get; set; }
        public string ImageMdUrl { get; set; }
        public string ImageLgUrl { get; set; }

        public static AuctionImageViewModel Create(ProductImage image)
        {
            return new AuctionImageViewModel
            {
                ImageSmUrl = image.ImageSmUrl,
                ImageMdUrl = image.ImageMdUrl,
                ImageLgUrl = image.ImageLgUrl
            };
        }
    }
}