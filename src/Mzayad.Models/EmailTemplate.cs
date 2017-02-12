using Mzayad.Models.Enum;
using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mzayad.Models.Enums;

namespace Mzayad.Models
{
    public class EmailTemplate : EntityBase, ILocalizable
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
