using System;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Web.Models
{
    public class DateRangeModel
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
