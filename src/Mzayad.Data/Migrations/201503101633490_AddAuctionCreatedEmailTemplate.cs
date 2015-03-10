using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctionCreatedEmailTemplate : DbMigration
    {
        public override void Up()
        {
            var template = new EmailTemplate
            {
                TemplateType = EmailTemplateType.AuctionCreated,
                Description = "Sent to subscribed users when an auction is created.",
                Subject = (new[]
                {
                    new LocalizedContent("en", "New Auction"),
                    new LocalizedContent("ar", "")
                }).Serialize(),
                Message = (new[]
                {
                    new LocalizedContent("en", 
                        @"<h1>Hi {FirstName}</h2>
                        <p>A new Mzayad auction has just been created for a product category you are subscribed to.</p>
                        <p><strong>Auction:</strong> <a href=''{AuctionUrl}''>{ProductName}</a></p>
                        <p>If you would like to review or change your subscription settings please visit <a href=''{NotificationsUrl}''>your notification settings</a>.</p>"),
                    new LocalizedContent("ar", "")
                }).Serialize()
            };

            Sql(string.Format("INSERT INTO [dbo].[EmailTemplates] ([TemplateType], [Description], [Subject], [Message]) VALUES ({0}, N'{1}', N'{2}', N'{3}');", (int)template.TemplateType, template.Description, template.Subject, template.Message));
        }
        
        public override void Down()
        {
        }
    }
}
