using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mzayad.Models
{
    public class Product : EntityBase, ILocalizable
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

        public int? SponsorId { get; set; }
        public string Notes { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
        [ForeignKey("SponsorId")]
        public virtual Sponsor Sponsor { get; set; } 

        public ProductImage MainImage()
        {
            if (!ProductImages.Any())
            {
                return new ProductImage
                {
                    ImageSmUrl = ProductImage.NoImageUrl,
                    ImageMdUrl = ProductImage.NoImageUrl,
                    ImageLgUrl = ProductImage.NoImageUrl
                };
            }

            return ProductImages.First();
        }
    }
}
