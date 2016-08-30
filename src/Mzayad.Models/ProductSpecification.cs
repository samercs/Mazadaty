using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class ProductSpecification: EntityBase, ILocalizable
    {
        [Key,Column(Order = 0)]
        public int ProductId { get; set; }
        [Key, Column(Order = 1)]
        public int SpecificationId { get; set; }
        
        [Required, Localized]
        public string Value { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("SpecificationId")]
        public virtual Specification Specification { get; set; }

    }
}
