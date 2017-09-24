using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Models.Enums;
using OrangeJetpack.Localization;

namespace Mazadaty.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddPrizeLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPrizeLogs",
                c => new
                {
                    PrizeId = c.Int(nullable: false),
                    UserId = c.String(nullable: false, maxLength: 128),
                    Hash = c.String(nullable: false, maxLength: 128),
                    IsDeleted = c.Boolean(nullable: false),
                    CreatedUtc = c.DateTime(nullable: false),
                    ModifiedUtc = c.DateTime(),
                    DeletedUtc = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => new { t.PrizeId, t.UserId, t.Hash })
                .ForeignKey("dbo.Prizes", t => t.PrizeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.PrizeId)
                .Index(t => t.UserId);

            var newTemplate = new EmailTemplate
            {
                TemplateType = EmailTemplateType.AdminWinningPrizeNotification,
                Description = "Sent to notify the admin for winning prize.",
                Subject = (new[]
                {
                    new LocalizedContent("en", "{0} - Winning User Notification"),
                    new LocalizedContent("ar", "{0} - Winning User Notification")
                }).Serialize(),
                Message = (new[]
                {
                    new LocalizedContent("en",
                        @"<p>Hi Administrator!</p><p>We send this email to you to let you now that a new winner in the {0} Site. </p>"
                        + @"<h2>Winner information :</h2>"
                        + @"<p>Name : {1}</p>"
                        + @"<p>Email : {2}</p>"
                        + @"<p>Phone Number : {3}</p>"
                        + @"<p>Winning Prize : {4}</p>"),
                    new LocalizedContent("ar",
                        @"<p>Hi Administrator!</p><p>We send this email to you to let you now that a new winner in the {0} Site. </p>"
                        + @"<h2>Winner information :</h2>"
                        + @"<p>Name : {1}</p>"
                        + @"<p>Email : {2}</p>"
                        + @"<p>Phone Number : {3}</p>"
                        + @"<p>Winning Prize : {4}</p>")
                }).Serialize()
            };
            Sql(string.Format("INSERT INTO [dbo].[EmailTemplates] ([TemplateType], [Description], [Subject], [Message]) VALUES ({0}, N'{1}', N'{2}', N'{3}');", (int)newTemplate.TemplateType, newTemplate.Description, newTemplate.Subject, newTemplate.Message));

        }

        public override void Down()
        {
            DropForeignKey("dbo.UserPrizeLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPrizeLogs", "PrizeId", "dbo.Prizes");
            DropIndex("dbo.UserPrizeLogs", new[] { "UserId" });
            DropIndex("dbo.UserPrizeLogs", new[] { "PrizeId" });
            DropTable("dbo.UserPrizeLogs");
            Sql(string.Format("DELETE FROM [dbo].[EmailTemplates] WHERE [TemplateType] = {0}", (int)EmailTemplateType.AdminWinningPrizeNotification));
        }
    }
}
