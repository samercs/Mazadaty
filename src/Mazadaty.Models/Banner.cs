using System.ComponentModel.DataAnnotations;
using Mazadaty.Models.Enums;
using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;

namespace Mazadaty.Models
{
    public class Banner : EntityBase, ILocalizable
    {
        public int BannerId { get; set; }
        [Localized, Required]
        public string Title { get; set; }
        [Localized, Required]
        public string SecondaryTitle { get; set; }
        public string OrginalImgUrl { get; set; }
        public string ImgSmUrl { get; set; }
        public string ImgMdUrl { get; set; }
        public string ImgLgUrl { get; set; }
        public string Url { get; set; }
        public BannerStatus Status { get; set; }

    }
}
