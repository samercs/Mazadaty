namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameAuctionWinningFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "WonUtc", c => c.DateTime());
            AddColumn("dbo.Auctions", "WonAmount", c => c.Decimal(precision: 18, scale: 3));
            DropColumn("dbo.Auctions", "ClosingUtc");
            DropColumn("dbo.Auctions", "ClosingPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Auctions", "ClosingPrice", c => c.Decimal(precision: 18, scale: 3));
            AddColumn("dbo.Auctions", "ClosingUtc", c => c.DateTime());
            DropColumn("dbo.Auctions", "WonAmount");
            DropColumn("dbo.Auctions", "WonUtc");
        }
    }
}
