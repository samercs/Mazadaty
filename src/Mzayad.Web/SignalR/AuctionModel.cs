using System;
using System.Collections.Generic;
using System.Linq;
using Mzayad.Models;
using Newtonsoft.Json;

namespace Mzayad.Web.SignalR
{
    [Serializable]
    internal class AuctionModel
    {
        public int AuctionId { get; set; }
        public int SecondsLeft { get; set; }
        public decimal? LastBidAmount { get; set; }
        
        [JsonIgnore]
        public DateTime StartUtc { get; set; }

        [JsonIgnore]
        public int Duration { get; set; }

        [JsonIgnore]
        public decimal BidIncrement { get; set; }

        public Queue<BidModel> Bids { get; set; } 

        public static AuctionModel Create(Auction auction)
        {
            var model = new AuctionModel
            {
                AuctionId = auction.AuctionId,
                StartUtc = auction.StartUtc,
                Duration = auction.Duration,
                BidIncrement = auction.BidIncrement,
                Bids = GetBidsQueue(auction.Bids)
            };

            model.LastBidAmount = model.Bids.Max(i => i.BidAmount);
            model.UpdateSecondsLeft();

            return model;
        }

        private static Queue<BidModel> GetBidsQueue(IEnumerable<Bid> bids)
        {
            return new Queue<BidModel>(bids
                .OrderByDescending(i => i.BidId)
                .Take(3)
                .OrderBy(i => i.BidId)
                .Select(BidModel.Create));
        }

        private void UpdateSecondsLeft()
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

    [Serializable]
    internal class BidModel
    {
        public string AvatarUrl { get; set; }
        public string UserName { get; set; }
        public decimal BidAmount { get; set; }

        internal static BidModel Create(Bid bid)
        {
            return new BidModel
            {
                AvatarUrl = bid.User.AvatarUrl,
                UserName = bid.User.UserName,
                BidAmount = bid.Amount
            };
        } 
    }
}
