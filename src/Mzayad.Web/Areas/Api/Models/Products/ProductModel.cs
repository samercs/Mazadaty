using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mzayad.Models;
using Mzayad.Web.Areas.Api.Models.Auctions;

namespace Mzayad.Web.Areas.Api.Models.Products
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public int AuctioId { get; set; }
        public string Name { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal? BuyNowPrice { get; set; }
        public string SponsorName { get; set; }
        public string VideoUrl { get; set; }
        public string MainImageUrl { get; set; }
        public string Description { get; set; }
        public IEnumerable<ProductSpecificationModel> Specifications { get; set; }
        public DateTime? AuctionCloseUtc { get; set; }
        public int? BuyNowQuntity { get; set; }

        public static ProductModel Create(Product product)
        {
            return new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                RetailPrice = product.RetailPrice,
                SponsorName = product.Sponsor?.Name ?? null,
                VideoUrl = product.VideoUrl,
                MainImageUrl = product.MainImage().ImageMdUrl,
                Description = product.Description,
                Specifications = product.ProductSpecifications?.Select(ProductSpecificationModel.Create)

            };
        }

        public static ProductModel Create(Auction auction)
        {
            return new ProductModel
            {
                ProductId = auction.ProductId,
                Name = auction.Title,
                RetailPrice = auction.Product.RetailPrice,
                BuyNowPrice = auction.BuyNowPrice,
                SponsorName = auction.Product.SponsorId.HasValue ? auction.Product.Sponsor.Name : null,
                MainImageUrl = auction.Product.MainImage().ImageMdUrl,
                AuctioId = auction.AuctionId,
                AuctionCloseUtc = auction.ClosedUtc,
                BuyNowQuntity = auction.BuyNowQuantity
            };
        }
    }

    public class ProductSpecificationModel
    {
        public string SpecificationName { get; set; }
        public string Value { get; set; }

        public static ProductSpecificationModel Create(ProductSpecification productSpecification)
        {
            return new ProductSpecificationModel
            {
                SpecificationName = productSpecification.Specification.Name,
                Value = productSpecification.Value
            };
        }
    }
}