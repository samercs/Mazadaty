using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;

namespace Mzayad.Models
{
    public class Avatar : EntityBase
    {
        public int AvatarId { get; set; }
        [DataType(DataType.Url)]
        public string Url { get; set; }
        public double SortOrder { get; set; }
        public bool IsPremium { get; set; }
        public int? Token { get; set; }
    }
}
