using System;
using System.Collections.Generic;
using System.Linq;
using Mzayad.Models;

namespace Mzayad.Web.Areas.Api.Models.Auctions
{
    public class AuctionModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public DateTime StartUtc { get; set; }
        public decimal RetailPrice { get; set; }
        public ProductModel Product { get; set; }

        public static AuctionModel Create(Auction auction)
        {
            return new AuctionModel
            {
                AuctionId = auction.AuctionId,
                Title = auction.Title,
                StartUtc = auction.StartUtc,
                RetailPrice = auction.RetailPrice,
                Product = ProductModel.Create(auction.Product)
            };
        }
    }

    public class ProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public ProductImageModel MainImage { get; set; }
        public IReadOnlyCollection<ProductImageModel> Images { get; set; } 

        public static ProductModel Create(Product product)
        {
            return new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                MainImage = ProductImageModel.Create(product.MainImage()),
                Images = product.ProductImages.Select(ProductImageModel.Create).ToList()
            };
        }
    }

    public class ProductImageModel
    {
        public string ImageSmUrl { get; set; }
        public string ImageMdUrl { get; set; }
        public string ImageLgUrl { get; set; }

        public static ProductImageModel Create(ProductImage image)
        {
            return new ProductImageModel
            {
                ImageSmUrl = image.ImageSmUrl,
                ImageMdUrl = image.ImageMdUrl,
                ImageLgUrl = image.ImageLgUrl
            };
        }
    }
}
