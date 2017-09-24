namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctionTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "Title", c => c.String(nullable: false));

            Sql("update dbo.Auctions set Title = (select Name from dbo.Products where dbo.Products.ProductId = dbo.Auctions.ProductId);");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "Title");
        }
    }
}
