using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderShippedEmailTemplate : DbMigration
    {
        public override void Up()
        {
            var template = new EmailTemplate
            {
                TemplateType = EmailTemplateType.OrderShipped,
                Description = "Sent to users when an order has shipped.",
                Subject = (new[]
                {
                    new LocalizedContent("en", "Order Shipped"),
                    new LocalizedContent("ar", "")
                }).Serialize(),
                Message = (new[]
                {
                    new LocalizedContent("en", 
                        @"<h1>Hi {FirstName}</h2>
                        <p>We just want to let you know your order is now out for delivery, you should receive it very soon.</p>"),
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
