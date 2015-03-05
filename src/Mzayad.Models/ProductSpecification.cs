using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    public class ProductSpecification: ModelBase, ILocalizable
    {
        [Key,Column(Order = 0)]
        public int ProductId { get; set; }
        [Key, Column(Order = 1)]
        public int SpecificationId { get; set; }
        
        [Required, StringLength(50), Localized]
        public string Value { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("SpecificationId")]
        public virtual Specification Specification { get; set; }

    }
}
