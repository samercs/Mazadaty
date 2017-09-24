namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSubscriptionPriceFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "PriceCurrency", c => c.Decimal(precision: 18, scale: 3));
            AddColumn("dbo.Subscriptions", "PriceTokens", c => c.Int());
            DropColumn("dbo.Subscriptions", "PurchasePrice");
            DropColumn("dbo.Subscriptions", "PurchaseTokens");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subscriptions", "PurchaseTokens", c => c.Int());
            AddColumn("dbo.Subscriptions", "PurchasePrice", c => c.Decimal(precision: 18, scale: 3));
            DropColumn("dbo.Subscriptions", "PriceTokens");
            DropColumn("dbo.Subscriptions", "PriceCurrency");
        }
    }
}
