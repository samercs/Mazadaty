using Microsoft.AspNet.Identity;
using Mzayad.Core.Formatting;
using Mzayad.Models;
using Mzayad.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Web.Models.Home
{
    public class IndexViewModel
    {
        public IReadOnlyCollection<AuctionViewModel> ClosedAuctions { get; set; }
        public IReadOnlyCollection<AuctionViewModel> LiveAuctions { get; set; }
        public IReadOnlyCollection<int> LiveAuctionIds { get; set; } 

        public IndexViewModel(IEnumerable<Auction> auctions, IEnumerable<Auction> closedAuctions)
        {
            ClosedAuctions = closedAuctions.Select(AuctionViewModel.Create).ToList();
            
            LiveAuctions = auctions.Where(i => i.IsLive())
                .Select(AuctionViewModel.Create)
                .ToList();
            
            LiveAuctionIds = LiveAuctions.Select(i => i.AuctionId).ToList();
        }
    }

    public class AuctionViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string RetailPrice { get; set; }
        public string BuyNowPrice { get; set; }
        public bool BuyNowEnabled { get; set; }

        public string WonByAmount { get; set; }
        public string WonByAvatarUrl { get; set; }
        public string WonByUserName { get; set; }

        public decimal? LastBidAmount { get; set; }
        public string LastBidUser { get; set; }
        public DateTime StartUtc { get; set; }
        public AuctionImageViewModel MainImage { get; set; }
        public IReadOnlyCollection<AuctionImageViewModel> Images { get; set; }
        public IReadOnlyCollection<ProductSpecificationViewModel> Specifications { get; set; }

        public static AuctionViewModel Create(Auction auction)
        {
            var viewModel = new AuctionViewModel
            {
                AuctionId = auction.AuctionId,
                Title = auction.Title, 
                Status = GetStatus(auction),
                Description = auction.Product.Description,
                RetailPrice = CurrencyFormatter.Format(auction.Product.RetailPrice),
                BuyNowPrice = CurrencyFormatter.Format(auction.BuyNowPrice),
                BuyNowEnabled = auction.BuyNowAvailable(),
                LastBidAmount = auction.WonAmount,
                LastBidUser = GetWonByUserName(auction.WonByUser),
                StartUtc = auction.StartUtc,
                MainImage = AuctionImageViewModel.Create(auction.Product.MainImage()),
                Images = GetImages(auction),
                Specifications = GetSpecifications(auction.Product.ProductSpecifications)
            };

            if (auction.WonByUser != null)
            {
                viewModel.WonByAmount = CurrencyFormatter.Format(auction.WonAmount);
                viewModel.WonByUserName = auction.WonByUser.UserName;
                viewModel.WonByAvatarUrl = auction.WonByUser.AvatarUrl;
            }

            return viewModel;
        }

        private static string GetWonByUserName(IUser<string> user)
        {
            return user == null ? "" : user.UserName;
        }

        private static IReadOnlyCollection<ProductSpecificationViewModel> GetSpecifications(IEnumerable<ProductSpecification> specifications)
        {
            return specifications.Select(specification => new ProductSpecificationViewModel
            {
                Name = specification.Specification.Name,
                Value = specification.Value
            }).ToList();
        }

        private static string GetStatus(Auction auction)
        {
            if (auction.Status == AuctionStatus.Closed)
            {
                return RenderStatus.Closed.ToString();
            }
            
            return auction.IsLive() 
                ? RenderStatus.Live.ToString() 
                : RenderStatus.Upcoming.ToString();
        }

        private static IReadOnlyCollection<AuctionImageViewModel> GetImages(Auction auction)
        {
            if (!auction.Product.ProductImages.Any())
            {
                return new[]
                {
                    new AuctionImageViewModel
                    {
                        ImageSmUrl = ProductImage.NoImageUrl,
                        ImageMdUrl = ProductImage.NoImageUrl,
                        ImageLgUrl = ProductImage.NoImageUrl
                    }
                };
            }

            return auction.Product.ProductImages.Select(AuctionImageViewModel.Create).ToList();
        }

        private enum RenderStatus { Live, Closed, Upcoming }
    }

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

    public class ProductSpecificationViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
