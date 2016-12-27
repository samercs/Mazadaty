using System.Collections.Generic;
using Mzayad.Models.Enums;
using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class Prize : EntityBase, ILocalizable
    {
        public int PrizeId { get; set; }
        [Required]
        public PrizeType PrizeType { get; set; }
        [Required, Localized]
        public string Title { get; set; }
        public int? Limit { get; set; }
        [Range(typeof(decimal), "0", "1")]
        public decimal Weight { get; set; }
        public int? ProductId { get; set; }
        public int? SubscriptionDays { get; set; }
        public PrizeStatus Status { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public virtual ICollection<UserPrizeLog> PrizeLogs { get; set; }

    }
}