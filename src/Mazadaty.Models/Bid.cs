using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mazadaty.Models.Enums;

namespace Mazadaty.Models
{
    public class Bid : EntityBase
    {
        public int BidId { get; set; }

        [Required, StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public int AuctionId { get; set; }

        public decimal Amount { get; set; }

        public int SecondsLeft { get; set; }

        [Required, StringLength(15)]
        public string UserHostAddress { get; set; }

        public  BidType Type { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        
        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        public const int ManualBidXp = 2;
        public const int AutoBidXp = 1;
    }
}
