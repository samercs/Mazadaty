using System;
using Microsoft.Azure.WebJobs;
using Mzayad.Client;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using OrangeJetpack.Services.Models;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;


namespace Mzayad.AutoBidNotifier
{
    public class Functions
    {
        private readonly AutoBidService _autoBidService;
        private readonly EmailTemplateService _emailTemplateService;

        public Functions()
        {
            _autoBidService = new AutoBidService(new DataContextFactory());
            _emailTemplateService = new EmailTemplateService(new DataContextFactory());
        }

        [NoAutomaticTrigger]
        public async Task NotifyAutoBidUsers(TextWriter log)
        {
            log.WriteLine("Running Functions.NotifyAutoBidUsers()...");
            var sw = Stopwatch.StartNew();

            var autoBids = await _autoBidService.GetAutoBiddersForNotification();
            foreach (var autoBid in autoBids)
            {
                await SendEmail(autoBid);
            }

            log.WriteLine($"Finished Functions.NotifyAutoBidUsers() in {sw.ElapsedMilliseconds} ms.");
        }

        private async Task SendEmail(AutoBid autoBid)
        {
            var template = await _emailTemplateService.GetByTemplateType(EmailTemplateType.AutoBidStart, "en");
            if (template == null)
            {
                return;
            }

            var timeToStart = autoBid.Auction.StartUtc.Subtract(DateTime.UtcNow).TotalMinutes;
            var email = new Email
            {
                ToAddress = autoBid.User.Email,
                Subject = template.Subject,
                Message = string.Format(template.Message, autoBid.User.FirstName, autoBid.Auction.Title, Convert.ToInt32(timeToStart))
            };

            await new EmailClient(email).Send();

        }
    }
}
