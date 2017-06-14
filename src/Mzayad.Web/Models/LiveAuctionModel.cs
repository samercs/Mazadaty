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
        public decimal LastBidAmount { get; set; }
        public string LastBidUserId { get; set; }

        public DateTime StartUtc;
        public int Duration;
        public decimal BidIncrement;
        public bool AutoBidEnabled { get; set; }

        public LinkedList<BidModel> Bids { get; set; }

        public static LiveAuctionModel Create(Auction auction)
        {
            var model = new LiveAuctionModel
            {
                AuctionId = auction.AuctionId,
                StartUtc = auction.StartUtc,
                Duration = auction.Duration,
                BidIncrement = auction.BidIncrement,
                Bids = BidModel.Create(auction.Bids),
                AutoBidEnabled = auction.AutoBidEnabled
            };

            var lastBid = model.Bids.FirstOrDefault();
            if (lastBid != null)
            {
                model.LastBidAmount = lastBid.BidAmount;
                model.LastBidUserId = lastBid.UserName;
            }

            model.UpdateSecondsLeft();

            return model;
        }

        internal int GetSecondsList()
        {
            return (int)Math.Floor(StartUtc.AddMinutes(Duration).Subtract(DateTime.UtcNow).TotalSeconds);
        }

        internal void UpdateSecondsLeft()
        {
            SecondsLeft = GetSecondsList();
        }

        public BidModel AddBid(Bid bid)
        {
            LastBidAmount = bid.Amount;
            LastBidUserId = bid.UserId;

            var bidModel = new BidModel
            {
                AvatarUrl = bid.User.AvatarUrl,
                UserId = bid.UserId,
                UserName = bid.User.UserName,
                BidAmount = bid.Amount
            };

            Bids.AddFirst(bidModel);

            while (Bids.Count > 3)
            {
                Bids.RemoveLast();
            }

            var secondsLeft = GetSecondsList();
            if (secondsLeft < 12)
            {
                StartUtc = StartUtc.AddSeconds(12 - secondsLeft);
            }

            return bidModel;
        }

        public decimal GetNewBidAmount()
        {
            return LastBidAmount + BidIncrement;
        }
    }
}