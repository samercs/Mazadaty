using Mzayad.Models.Enum;
using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mzayad.Models.Enums;

namespace Mzayad.Models
{
    public class Auction : EntityBase, ILocalizable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuctionId { get; set; }

        [Required, Localized]
        public string Title { get; set; }
        
        [Required]
        public DateTime StartUtc { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public AuctionStatus Status { get; set; }
        [Required]
        public decimal RetailPrice { get; set; }
        [Required]
        public decimal BidIncrement { get; set; }
        [Required]
        public int Duration { get; set; }

        public decimal? MaximumBid { get; set; }
        public bool BuyNowEnabled { get; set; }
        public decimal? BuyNowPrice { get; set; }
        public int? BuyNowQuantity { get; set; }
        public decimal? ReservePrice { get; set; }
        
        [StringLength(128)]
        public string CreatedByUserId { get; set; }

        public DateTime? ClosedUtc { get; set; }
        
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        [StringLength(128)]
        public string WonByUserId { get; set; }

        [ForeignKey("WonByUserId")]
        public virtual ApplicationUser WonByUser { get; set; }

        public decimal? WonAmount { get; set; }
        public int? WonByBidId { get; set; }
        
        public virtual Product Product { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        public bool IsLive()
        {
            return Status != AuctionStatus.Closed && StartUtc <= DateTime.UtcNow;
        }

        public bool BuyNowAvailable()
        {
            return BuyNowEnabled && BuyNowQuantity > 0 && Product.Quantity > 0;
        }
    }
}
