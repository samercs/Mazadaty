namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAutoBid : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AutoBids",
                c => new
                    {
                        AutoBidId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        AuctionId = c.Int(nullable: false),
                        MaxBid = c.Decimal(nullable: false, precision: 18, scale: 3),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AutoBidId)
                .ForeignKey("dbo.Auctions", t => t.AuctionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AuctionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AutoBids", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AutoBids", "AuctionId", "dbo.Auctions");
            DropIndex("dbo.AutoBids", new[] { "AuctionId" });
            DropIndex("dbo.AutoBids", new[] { "UserId" });
            DropTable("dbo.AutoBids");
        }
    }
}
