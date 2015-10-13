using Mzayad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Web.Models
{
    [Serializable]
    internal class LiveAuctionModel
    {
        public int AuctionId { get; set; }
        public int SecondsLeft { get; set; }
        public decimal? LastBidAmount { get; set; }

        public DateTime StartUtc;
        public int Duration;
        public decimal BidIncrement;

        public Queue<BidModel> Bids { get; set; }

        //[JsonProperty("bids")]
        //public BidModel[] BidsReversed
        //{
        //    get
        //    {
        //        if (Bids == null || !Bids.Any())
        //        {
        //            return new BidModel[0];
        //        }

        //        return Bids.OrderByDescending(i => i.BidAmount).ToArray();
        //    }
        //}

        public static LiveAuctionModel Create(Auction auction)
        {
            var model = new LiveAuctionModel
            {
                AuctionId = auction.AuctionId,
                StartUtc = auction.StartUtc,
                Duration = auction.Duration,
                BidIncrement = auction.BidIncrement,
                Bids = BidModel.Create(auction.Bids)
            };

            model.LastBidAmount = model.Bids.Any() ? model.Bids.Max(i => i.BidAmount) : 0;
            model.UpdateSecondsLeft();

            return model;
        }

        internal void UpdateSecondsLeft()
        {
            SecondsLeft = (int)Math.Floor(StartUtc.AddMinutes(Duration).Subtract(DateTime.UtcNow).TotalSeconds);
        }

        public BidModel AddBid(ApplicationUser user)
        {
            LastBidAmount = LastBidAmount.GetValueOrDefault(0) + BidIncrement;
            
            var bid = new BidModel
            {
                AvatarUrl = user.AvatarUrl,
                UserName = user.UserName,
                BidAmount = LastBidAmount.Value
            };

            Bids.Enqueue(bid);

            while (Bids.Count > 3)
            {
                Bids.Dequeue();
            }

            if (SecondsLeft < 12)
            {
                SecondsLeft = 12;
            }

            return bid;
        }
    }
}