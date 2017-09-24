namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubscriptionPriceAndSortOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "PurchasePrice", c => c.Decimal(precision: 18, scale: 3));
            AddColumn("dbo.Subscriptions", "PurchaseTokens", c => c.Int());
            AddColumn("dbo.Subscriptions", "SortOrder", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscriptions", "SortOrder");
            DropColumn("dbo.Subscriptions", "PurchaseTokens");
            DropColumn("dbo.Subscriptions", "PurchasePrice");
        }
    }
}
