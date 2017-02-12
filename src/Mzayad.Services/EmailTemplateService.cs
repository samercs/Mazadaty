using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class EmailTemplateService : ServiceBase
    {
        public EmailTemplateService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<EmailTemplate>> GetAll()
        {
            using (var dc = DataContext())
            {
                return await dc.EmailTemplates.ToListAsync();
            }
        }

        public async Task<EmailTemplate> GetByTemplateType(EmailTemplateType emailTemplateType)
        {
            using (var dc = DataContext())
            {
                return await dc.EmailTemplates.SingleOrDefaultAsync(i => i.TemplateType == emailTemplateType);
            }
        }

        public async Task<EmailTemplate> GetByTemplateType(EmailTemplateType emailTemplateType, string languageCode)
        {
            using (var dc = DataContext())
            {
                var template = await dc.EmailTemplates.SingleOrDefaultAsync(i => i.TemplateType == emailTemplateType);

                return template.Localize(languageCode, i => i.Subject, i => i.Message);
            }
        }

        public async Task<EmailTemplate> Save(EmailTemplate emailTemplate)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(emailTemplate);
                await dc.SaveChangesAsync();
                return emailTemplate;
            }
        }
    }
}
