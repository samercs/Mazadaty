namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreatedByUserId_Product_Auction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Products", "CreatedByUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Auctions", "CreatedByUserId");
            CreateIndex("dbo.Products", "CreatedByUserId");
            AddForeignKey("dbo.Auctions", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Products", "CreatedByUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Auctions", "CreatedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Products", new[] { "CreatedByUserId" });
            DropIndex("dbo.Auctions", new[] { "CreatedByUserId" });
            DropColumn("dbo.Products", "CreatedByUserId");
            DropColumn("dbo.Auctions", "CreatedByUserId");
        }
    }
}
