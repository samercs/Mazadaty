using System;
using System.Collections.Generic;
using Mzayad.Models;
using Newtonsoft.Json;

namespace Mzayad.Web.SignalR
{
    [Serializable]
    internal class Auction
    {
        [JsonProperty("id")]
        public int AuctionId { get; set; }

        [JsonProperty("t")]
        public int SecondsLeft { get; set; }

        [JsonProperty("a")]
        public decimal? LastBidAmount { get; set; }
        
        [JsonProperty("u")]
        public string LastBidderName { get; set; }

        [JsonIgnore]
        public DateTime StartUtc { get; set; }

        [JsonIgnore]
        public int Duration { get; set; }

        [JsonIgnore]
        public decimal BidIncrement { get; set; }

        public Queue<Bid> Bids { get; set; } 

        public Auction(Mzayad.Models.Auction auction)
        {
            AuctionId = auction.AuctionId;
            StartUtc = auction.StartUtc;
            Duration = auction.Duration;
            BidIncrement = auction.BidIncrement;
            Bids = new Queue<Bid>(3);
            
            UpdateSecondsLeft();
        }

        private void UpdateSecondsLeft()
        {
            SecondsLeft = (int)Math.Floor(StartUtc.AddMinutes(Duration).Subtract(DateTime.UtcNow).TotalSeconds);
        }

        public Bid AddBid(ApplicationUser user)
        {
            LastBidAmount = (LastBidAmount ?? 0) + BidIncrement;
            
            var bid = new Bid
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
    internal class Bid
    {
        public string AvatarUrl { get; set; }
        public string UserName { get; set; }
        public decimal BidAmount { get; set; }
    }
}
