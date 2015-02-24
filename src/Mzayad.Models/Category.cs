using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    [Serializable]
    public class Category : ModelBase, ILocalizable
    {
        public int CategoryId { get; set; }

        [Required, Localized]
        public string Name { get; set; }

        [Required, StringLength(50)]
        [Index("IX_Slug", 1, IsUnique = true)]
        public string Slug { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Category Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
