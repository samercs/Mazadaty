namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameAuctionWonUtcToClosedUtc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "ClosedUtc", c => c.DateTime());
            DropColumn("dbo.Auctions", "WonUtc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Auctions", "WonUtc", c => c.DateTime());
            DropColumn("dbo.Auctions", "ClosedUtc");
        }
    }
}
