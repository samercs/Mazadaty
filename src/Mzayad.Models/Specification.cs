using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    public class Specification : ModelBase, ILocalizable
    {
        public int SpecificationId { get; set; }

        [Required, Localized]
        public string Name { get; set; }
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
        
    }
}
