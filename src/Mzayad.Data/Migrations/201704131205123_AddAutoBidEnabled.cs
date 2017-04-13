namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddAutoBidEnabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "AutoBidEnabled", c => c.Boolean(nullable: false, defaultValue: true));
        }

        public override void Down()
        {
            DropColumn("dbo.Auctions", "AutoBidEnabled");
        }
    }
}
