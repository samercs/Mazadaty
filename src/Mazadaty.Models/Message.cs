using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Mazadaty.Models
{
    public class Message : EntityBase
    {
        [Key]
        public int MessageId { get; set; }

        [Required, StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;

        [Required, StringLength(128)]
        public string ReceiverId { get; set; }

        [Required]
        public bool IsNew { get; set; } = true;

        [NotMapped]
        public string Summary
        {
            get
            {
                var regex = new Regex("<[a-z/!](.|\n)*?>");
                var summary = regex.Replace(Body, "");
                if (summary.Length > 150)
                {
                    return summary.Substring(0, 150);
                }
                return summary;
            }
        }

        public virtual ApplicationUser User { get; set; }
    }
}
