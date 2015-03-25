using System;
using Newtonsoft.Json;

namespace Mzayad.Web.SignalR
{
    internal class Auction
    {
        [JsonProperty("id")]
        public int AuctionId { get; set; }
        public double SecondsLeft { get; set; }
        public decimal? LastBidAmount { get; set; }
        public string LastBidderName { get; set; }

        [JsonIgnore]
        public DateTime StartUtc { get; set; }

        [JsonIgnore]
        public int Duration { get; set; }    
    }
}
