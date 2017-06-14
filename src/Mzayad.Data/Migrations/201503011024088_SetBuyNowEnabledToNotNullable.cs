namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetBuyNowEnabledToNotNullable : DbMigration
    {
        public override void Up()
        {
            Sql("update dbo.Auctions set BuyNowEnabled = 0 where BuyNowEnabled is null;");
            AlterColumn("dbo.Auctions", "BuyNowEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Auctions", "BuyNowEnabled", c => c.Boolean());
        }
    }
}
