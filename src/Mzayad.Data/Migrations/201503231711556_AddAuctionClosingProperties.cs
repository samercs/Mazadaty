namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctionClosingProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "WonByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Auctions", "ClosingUtc", c => c.DateTime());
            AddColumn("dbo.Auctions", "ClosingPrice", c => c.Decimal(precision: 18, scale: 3));
            CreateIndex("dbo.Auctions", "WonByUserId");
            AddForeignKey("dbo.Auctions", "WonByUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "WonByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "WonByUserId" });
            DropColumn("dbo.Auctions", "ClosingPrice");
            DropColumn("dbo.Auctions", "ClosingUtc");
            DropColumn("dbo.Auctions", "WonByUserId");
        }
    }
}
