using System.ComponentModel.DataAnnotations;

namespace Mzayad.Models
{
    public class Avatar : ModelBase
    {
        public int AvatarId { get; set; }
        [DataType(DataType.Url)]
        public string Url { get; set; }
        public double SortOrder { get; set; }
    }
}
