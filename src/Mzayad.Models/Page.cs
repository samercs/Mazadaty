using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Models.Enums;
using OrangeJetpack.Base.Data;

namespace Mzayad.Models
{
    public class Page: EntityBase
    {
        public int PageId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Index("IX_PageTag", IsUnique = true)]
        [StringLength(20)]
        public string PageTag { get; set; }
        [DefaultValue(PageStatus.Pupblic)]
        public PageStatus Status { get; set; }
        [Required, StringLength(128)]
        public string UserId { get; set; }
        public string Author { get; set; }
    }
}
