using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Models
{
    public class Specification : EntityBase, ILocalizable
    {
        public int SpecificationId { get; set; }

        [Required, Localized]
        public string Name { get; set; }
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
        
    }
}
