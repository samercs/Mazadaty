using OrangeJetpack.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class Product : ModelBase , ILocalizable
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required, Localized]
        public string Name { get; set; }

        [Required,Localized]
        [UIHint("TextArea")]
        public string Description { get; set; }

        public decimal RetailPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Quantity { get; set; }

        public string VideoUrl { get; set; }

        [StringLength(128)]
        public string CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
    }
}
