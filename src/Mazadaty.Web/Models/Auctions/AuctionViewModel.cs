using Microsoft.AspNet.Identity;
using Mazadaty.Core.Formatting;
using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Web.Core.Enums;
using System;

namespace Mazadaty.Web.Models.Auctions
{
    public class AuctionViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string RetailPrice { get; set; }
        public string BuyNowPrice { get; set; }
        public bool BuyNowEnabled { get; set; }

        public string WonByAmount { get; set; }
        public string WonByAvatarUrl { get; set; }
        public string WonByUserName { get; set; }

        public decimal? LastBidAmount { get; set; }
        public string LastBidUser { get; set; }
        public DateTime StartUtc { get; set; }
        public string ImageUrl { get; set; }
        public string SponsorName { get; set; }
        public int ProductId { get; set; }
        public AuctionType Type { get; set; }
        public decimal? MaximumBid { get; set; }
        public decimal BidIncrement { get; set; }
        public int Duration { get; set; }

        public bool AutoBidEnabled { get; set; }

        public AuctionStatus AuctionStatus { get; set; }

        public string CountryList { get; set; }
        public DateTime? ClosedUtc { get; set; }

        public static AuctionViewModel Create(Auction auction)
        {
            var viewModel = new AuctionViewModel
            {
                AuctionId = auction.AuctionId,
                Title = auction.Title,
                Status = GetStatus(auction),
                RetailPrice = CurrencyFormatter.Format(auction.Product.RetailPrice),
                BuyNowPrice = CurrencyFormatter.Format(auction.BuyNowPrice),
                BuyNowEnabled = auction.BuyNowAvailable(),
                LastBidAmount = auction.WonAmount,
                LastBidUser = GetWonByUserName(auction.WonByUser),
                StartUtc = auction.StartUtc,
                ImageUrl = auction.Product.MainImage().ImageMdUrl,
                SponsorName = auction.Product.Sponsor != null ? auction.Product.Sponsor.Name : string.Empty,
                ProductId = auction.ProductId,
                CountryList = auction.CountryList,
                MaximumBid = auction.MaximumBid,
                BidIncrement = auction.BidIncrement,
                Duration = auction.Duration,
                AuctionStatus = auction.Status,
                AutoBidEnabled = auction.AutoBidEnabled,
                ClosedUtc = auction.ClosedUtc
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

        private static string GetStatus(Auction auction)
        {
            if (auction.Status == AuctionStatus.Closed)
            {
                return AuctionType.Closed.ToString();
            }

            return auction.IsLive()
                ? AuctionType.Live.ToString()
                : AuctionType.Upcoming.ToString();
        }
    }
}
