using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAutoBidStartEmailTemplate : DbMigration
    {
        public override void Up()
        {
            var emailTemplate = new EmailTemplate
            {
                TemplateType = EmailTemplateType.AutoBidStart,
                Description = "Sent when auction about to start.",
                Subject = (new[]
                {
                    new LocalizedContent("en", @"Auction Start "),
                    new LocalizedContent("ar", @"")
                }).Serialize(),
                Message = (new[]
                {
                    new LocalizedContent("en",
                        @"<h2>Hi {0}</h2><p> This is a notification to let you know that Auction <strong>{1}</strong> will start in {2} minutes.</p>"),
                    new LocalizedContent("ar", @"")
                }).Serialize()
            };

            Sql($"INSERT INTO [dbo].[EmailTemplates] ([TemplateType], [Description], [Subject], [Message]) VALUES (N'{(int)emailTemplate.TemplateType}', N'{emailTemplate.Description}', N'{emailTemplate.Subject}', N'{emailTemplate.Message}');");
        }
        
        public override void Down()
        {
            Sql("Delete from EmailTemplates where TemplateType=10 ");
        }
    }
}
