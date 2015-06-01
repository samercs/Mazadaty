namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubscriptionLog : DbMigration
    {
        public override void Up()
        {
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
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ModifiedByUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SubscriptionLogs", "ModifiedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.SubscriptionLogs", new[] { "ModifiedByUserId" });
            DropIndex("dbo.SubscriptionLogs", new[] { "UserId" });
            DropTable("dbo.SubscriptionLogs");
        }
    }
}
