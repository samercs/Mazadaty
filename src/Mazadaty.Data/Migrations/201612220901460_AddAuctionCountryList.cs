namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctionCountryList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "CountryList", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "CountryList");
        }
    }
}
