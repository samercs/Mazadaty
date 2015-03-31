using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class Bid : ModelBase
    {
        public int BidId { get; set; }

        [Required, StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
        public int SecondsLeft { get; set; }

        [Required, ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        
        [Required, ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }
    }
}
