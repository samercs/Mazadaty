namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddWaiveShippingCostToProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "WaiveShippingCost", c => c.Boolean(nullable: false, defaultValue: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Products", "WaiveShippingCost");
        }
    }
}
