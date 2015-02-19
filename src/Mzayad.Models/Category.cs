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
    [Serializable]
    public class Category : ModelBase, ILocalizable
    {
        public int CategoryId { get; set; }

        [Required, Localized]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Category Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; }
    }
}
