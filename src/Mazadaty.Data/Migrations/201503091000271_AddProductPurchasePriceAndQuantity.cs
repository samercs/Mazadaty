namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductPurchasePriceAndQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "PurchasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 3));
            AddColumn("dbo.Products", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Quantity");
            DropColumn("dbo.Products", "PurchasePrice");
        }
    }
}
