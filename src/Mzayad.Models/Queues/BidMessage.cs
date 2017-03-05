using Mzayad.Models.Enums;

namespace Mzayad.Models.Queues
{
    public class BidMessage : QueueMessageBase
    {
        public int BidId { get; set; }
        public int AuctionId { get; set; }
        public string UserId { get; set; }
        public BidType Type { get; set; }

        public BidMessage()
        {
            
        }

        public BidMessage(Bid bid)
        {
            BidId = bid.BidId;
            AuctionId = bid.AuctionId;
            UserId = bid.UserId;
            Type = bid.Type;
        }
    }
}
