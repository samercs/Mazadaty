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

        public void UpdateSecondsLeft()
        {
            SecondsLeft = (int)Math.Floor(StartUtc.AddMinutes(Duration).Subtract(DateTime.UtcNow).TotalSeconds);
        }
    }
}
