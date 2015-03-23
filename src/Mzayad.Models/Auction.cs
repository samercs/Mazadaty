using Mzayad.Models.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    public class Auction : ModelBase, ILocalizable
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

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual Product Product { get; set; }
    }
}
