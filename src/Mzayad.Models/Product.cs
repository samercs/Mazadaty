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

        public string VideoUrl { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }

    }
}
