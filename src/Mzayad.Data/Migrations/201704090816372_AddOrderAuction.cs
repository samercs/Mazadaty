namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderAuction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "AuctionId", c => c.Int());
            CreateIndex("dbo.OrderItems", "AuctionId");
            AddForeignKey("dbo.OrderItems", "AuctionId", "dbo.Auctions", "AuctionId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "AuctionId", "dbo.Auctions");
            DropIndex("dbo.OrderItems", new[] { "AuctionId" });
            DropColumn("dbo.OrderItems", "AuctionId");
        }
    }
}
