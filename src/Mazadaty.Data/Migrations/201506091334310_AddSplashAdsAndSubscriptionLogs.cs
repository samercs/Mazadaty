namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSplashAdsAndSubscriptionLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SplashAds",
                c => new
                    {
                        SplashAdId = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Weight = c.Int(nullable: false),
                        SortOrder = c.Double(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SplashAdId);
            
            CreateTable(
                "dbo.SubscriptionLogs",
                c => new
                    {
                        SubscriptionLogId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ModifiedByUserId = c.String(nullable: false, maxLength: 128),
                        OriginalSubscriptionUtc = c.DateTime(),
                        ModifiedSubscriptionUtc = c.DateTime(nullable: false),
                        UserHostAddress = c.String(nullable: false, maxLength: 15),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionLogId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.ModifiedByUserId);
            
            AddColumn("dbo.AspNetUsers", "SubscriptionUtc", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SubscriptionLogs", "ModifiedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.SubscriptionLogs", new[] { "ModifiedByUserId" });
            DropIndex("dbo.SubscriptionLogs", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "SubscriptionUtc");
            DropTable("dbo.SubscriptionLogs");
            DropTable("dbo.SplashAds");
        }
    }
}
