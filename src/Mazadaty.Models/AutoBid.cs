using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models
{
    public class AutoBid : EntityBase
    {
        public int AutoBidId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }
        public int AuctionId { get; set; }

        public decimal MaxBid { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
        public virtual Auction Auction { get; set; }
    }
}
