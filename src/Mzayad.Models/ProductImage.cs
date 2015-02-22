using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Models
{
    public class ProductImage : ModelBase
    {
        public int ProductImageId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public double SortOrder { get; set; }

        public string ImageSmUrl { get; set; }
        public string ImageMdUrl { get; set; }
        public string ImageLgUrl { get; set; }

        [NotMapped]
        public const string NoImageUrl = "/content/images/no-image.png";

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
