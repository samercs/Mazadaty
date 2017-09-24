namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctionReservePrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "ReservePrice", c => c.Decimal(precision: 18, scale: 3));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "ReservePrice");
        }
    }
}
