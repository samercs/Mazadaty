namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryNotification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoryNotifications",
                c => new
                    {
                        CategoryNotificationId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        CategoryId = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryNotificationId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CategoryNotifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CategoryNotifications", "CategoryId", "dbo.Categories");
            DropIndex("dbo.CategoryNotifications", new[] { "CategoryId" });
            DropIndex("dbo.CategoryNotifications", new[] { "UserId" });
            DropTable("dbo.CategoryNotifications");
        }
    }
}
