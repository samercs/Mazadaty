using System;
using Newtonsoft.Json;

namespace Mzayad.Web.SignalR
{
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

        public Auction(Mzayad.Models.Auction auction)
        {
            AuctionId = auction.AuctionId;
            StartUtc = auction.StartUtc;
            Duration = auction.Duration;
            BidIncrement = auction.BidIncrement;
            
            UpdateSecondsLeft();
        }

        private void UpdateSecondsLeft()
        {
            SecondsLeft = (int)Math.Floor(StartUtc.AddMinutes(Duration).Subtract(DateTime.UtcNow).TotalSeconds);
        }

        public void AddBid(string username)
        {
            LastBidAmount = (LastBidAmount ?? 0) + BidIncrement;
            LastBidderName = username;

            if (SecondsLeft < 12)
            {
                SecondsLeft = 12;
            }
        }
    }
}
