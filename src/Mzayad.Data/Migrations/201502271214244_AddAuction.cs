namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuction : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        AuctionId = c.Int(nullable: false, identity: true),
                        StartUtc = c.DateTime(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        RetailPrice = c.Decimal(nullable: false, precision: 18, scale: 3),
                        BidIncrement = c.Decimal(nullable: false, precision: 18, scale: 3),
                        Duration = c.Int(nullable: false),
                        MaximumBid = c.Decimal(precision: 18, scale: 3),
                        BuyNowEnabled = c.Boolean(),
                        BuyNowPrice = c.Decimal(precision: 18, scale: 3),
                        BuyNowQuantity = c.Int(),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuctionId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "ProductId", "dbo.Products");
            DropIndex("dbo.Auctions", new[] { "ProductId" });
            DropTable("dbo.Auctions");
            
        }
    }
}
