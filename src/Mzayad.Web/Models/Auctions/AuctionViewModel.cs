using Microsoft.AspNet.Identity;
using Mzayad.Core.Formatting;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Web.Core.Enums;
using System;

namespace Mzayad.Web.Models.Auctions
{
    public class AuctionViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        //public string Description { get; set; }
        public string RetailPrice { get; set; }
        public string BuyNowPrice { get; set; }
        public bool BuyNowEnabled { get; set; }

        public string WonByAmount { get; set; }
        public string WonByAvatarUrl { get; set; }
        public string WonByUserName { get; set; }

        public decimal? LastBidAmount { get; set; }
        public string LastBidUser { get; set; }
        public DateTime StartUtc { get; set; }
        //public AuctionImageViewModel MainImage { get; set; }
        public string ImageUrl { get; set; }
        public string SponsorName { get; set; }
        public int ProductId { get; set; }
        public AuctionType Type { get; set; }
        public decimal? MaximumBid { get; set; }
        public decimal BidIncrement { get; set; }
        //public IReadOnlyCollection<AuctionImageViewModel> Images { get; set; }
        //public IReadOnlyCollection<ProductSpecificationViewModel> Specifications { get; set; }

        public string CountryList { get; set; }

        public static AuctionViewModel Create(Auction auction)
        {
            var viewModel = new AuctionViewModel
            {
                AuctionId = auction.AuctionId,
                Title = auction.Title,
                Status = GetStatus(auction),
                //Description = auction.Product.Description,
                RetailPrice = CurrencyFormatter.Format(auction.Product.RetailPrice),
                BuyNowPrice = CurrencyFormatter.Format(auction.BuyNowPrice),
                BuyNowEnabled = auction.BuyNowAvailable(),
                LastBidAmount = auction.WonAmount,
                LastBidUser = GetWonByUserName(auction.WonByUser),
                StartUtc = auction.StartUtc,
                //MainImage = AuctionImageViewModel.Create(auction.Product.MainImage()),
                ImageUrl = auction.Product.MainImage().ImageMdUrl,
                SponsorName = auction.Product.Sponsor != null ? auction.Product.Sponsor.Name : string.Empty,
                ProductId = auction.ProductId,
                CountryList = auction.CountryList,
                MaximumBid = auction.MaximumBid,
                BidIncrement = auction.BidIncrement
                //Images = GetImages(auction)
                //Specifications = GetSpecifications(auction.Product.ProductSpecifications)
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

        //private static IReadOnlyCollection<ProductSpecificationViewModel> GetSpecifications(IEnumerable<ProductSpecification> specifications)
        //{
        //    return specifications.Select(specification => new ProductSpecificationViewModel
        //    {
        //        Name = specification.Specification.Name,
        //        Value = specification.Value
        //    }).ToList();
        //}

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

        //private static IReadOnlyCollection<AuctionImageViewModel> GetImages(Auction auction)
        //{
        //    if (!auction.Product.ProductImages.Any())
        //    {
        //        return new[]
        //        {
        //            new AuctionImageViewModel
        //            {
        //                ImageSmUrl = ProductImage.NoImageUrl,
        //                ImageMdUrl = ProductImage.NoImageUrl,
        //                ImageLgUrl = ProductImage.NoImageUrl
        //            }
        //        };
        //    }

        //    return auction.Product.ProductImages.Select(AuctionImageViewModel.Create).ToList();
        //}

        private enum RenderStatus { Live, Closed, Upcoming }
    }
}