using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Models
{
    public class SessionLog : ModelBase
    {
        public int SessionLogId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string IP { get; set; }

        public string Browser { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
