namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctionWonByBid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "WonByBidId", c => c.Int());

            AddForeignKey("dbo.Auctions", "WonByBidId", "dbo.Bids", "BidId");
            CreateIndex("dbo.Auctions", "WonByBidId");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "WonByBidId");
        }
    }
}
