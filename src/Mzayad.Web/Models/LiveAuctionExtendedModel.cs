using System.Linq;
using Mzayad.Models;

namespace Mzayad.Web.Models
{
    internal class LiveAuctionExtendedModel : LiveAuctionModel
    {
        public string Title { get; set; }
        public decimal RetailPrice { get; set; }
        public string ImageUrl { get; set; }

        public new static LiveAuctionExtendedModel Create(Auction auction)
        {
            var model = new LiveAuctionExtendedModel
            {
                AuctionId = auction.AuctionId,         
                Title = auction.Title,
                ImageUrl = auction.Product.MainImage().ImageMdUrl,
                RetailPrice = auction.RetailPrice,
                StartUtc = auction.StartUtc,
                Duration = auction.Duration,
                BidIncrement = auction.BidIncrement,
                Bids = BidModel.Create(auction.Bids),  
            };

            model.LastBidAmount = model.Bids.Any() ? model.Bids.Max(i => i.BidAmount) : 0;
            model.UpdateSecondsLeft();

            return model;
        }
    }
}