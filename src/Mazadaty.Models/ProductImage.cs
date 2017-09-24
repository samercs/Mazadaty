using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models
{
    public class ProductImage : EntityBase
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
        public const string NoImageUrl = "//az712326.vo.msecnd.net/assets/no-image-512x512-635627099896729695.png";

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
