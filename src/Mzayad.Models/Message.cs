using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Models
{
    public class Message : EntityBase
    {
        [Key]
        public int MessageId { get; set; }

        [Required, StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public string Body { get; set; }

        [Required, StringLength(128)]
        public string ReceiverId { get; set; }

        [Required]
        public bool IsNew { get; set; } = true;
        public virtual ApplicationUser User { get; set; }
    }
}
