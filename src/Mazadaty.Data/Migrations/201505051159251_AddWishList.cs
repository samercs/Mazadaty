namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWishList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WishLists",
                c => new
                    {
                        WishListId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        NameEntered = c.String(nullable: false),
                        NameNormalized = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WishListId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AlterColumn("dbo.Subscriptions", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WishLists", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.WishLists", new[] { "UserId" });
            AlterColumn("dbo.Subscriptions", "Name", c => c.String());
            DropTable("dbo.WishLists");
        }
    }
}
