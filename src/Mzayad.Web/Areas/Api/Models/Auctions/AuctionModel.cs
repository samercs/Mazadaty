using System;
using System.Collections.Generic;
using System.Linq;
using Mzayad.Models;
using Mzayad.Models.Enums;

namespace Mzayad.Web.Areas.Api.Models.Auctions
{
    public class AuctionModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public decimal RetailPrice { get; set; }
        public string CountryList { get; set; }
        public AuctionStatus Status { get; set; }
        public DateTime StartUtc { get; set; }
        public decimal BidIncrement { get; set; }
        public int Duration { get; set; }
        public decimal? MaximumBid { get; set; }
        public bool BuyNowEnabled { get; set; }
        public decimal? BuyNowPrice { get; set; }
        public int? BuyNowQuantity { get; set; }
        public decimal? WonAmount { get; set; }
        public DateTime? ClosedUtc { get; set; }
        public ProductModel Product { get; set; }
        public UserModel WonByUser { get; set; }
        public IEnumerable<BidModel> RecentBids { get; set; }

        public static AuctionModel Create(Auction auction)
        {
            return new AuctionModel
            {
                AuctionId = auction.AuctionId,
                Title = auction.Title,
                RetailPrice = auction.RetailPrice,
                CountryList = auction.CountryList,
                Status = auction.Status,
                StartUtc = auction.StartUtc,
                BidIncrement = auction.BidIncrement,
                Duration = auction.Duration,
                MaximumBid = auction.MaximumBid,
                BuyNowEnabled = auction.BuyNowEnabled,
                BuyNowPrice = auction.BuyNowPrice,
                BuyNowQuantity = auction.BuyNowQuantity,
                WonAmount = auction.WonAmount,
                ClosedUtc = auction.ClosedUtc,
                Product = ProductModel.Create(auction.Product),
                WonByUser = string.IsNullOrWhiteSpace(auction.WonByUserId) ? null : UserModel.Create(auction.WonByUser),
                RecentBids = auction.Bids != null && auction.Bids.Any()
                    ? auction.Bids
                        .OrderByDescending(i => i.BidId)
                        .Take(3)
                        .Select(BidModel.Create)
                    : null
            };
        }
    }

    public class ProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string SponsorName { get; set; }

        public static ProductModel Create(Product product)
        {
            return new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ImageUrl = product.MainImage().ImageMdUrl,
                SponsorName = product.SponsorId.HasValue ? product.Sponsor.Name : string.Empty
            };
        }
}

public class BidModel
{
    public int BidId { get; set; }

    public int AuctionId { get; set; }

    public decimal Amount { get; set; }

    public UserModel User { get; set; }

    public static BidModel Create(Bid bid)
    {
        return new BidModel
        {
            BidId = bid.BidId,
            AuctionId = bid.AuctionId,
            Amount = bid.Amount,
            User = UserModel.Create(bid.User)
        };
    }
}

public class UserModel
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string AvatarUrl { get; set; }

    public static UserModel Create(ApplicationUser user)
    {
        return new UserModel
        {
            Id = user.Id,
            Username = user.UserName,
            AvatarUrl = user.AvatarUrl
        };
    }
}
}
