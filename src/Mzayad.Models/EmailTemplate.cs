using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    public class EmailTemplate : ModelBase, ILocalizable
    {
        public int EmailTemplateId { get; set; }

        [Index(IsUnique = true)]
        public EmailTemplateType TemplateType { get; set; }

        [Required]

        public string Description { get; set; }

        [Required, Localized, Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required, Localized, Display(Name = "Message")]
        [UIHint("TextArea")]
        public string Message { get; set; }
    }
}
