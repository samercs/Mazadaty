namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOrderItemForSubscription : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            AddColumn("dbo.OrderItems", "SubscriptionId", c => c.Int());
            AlterColumn("dbo.OrderItems", "ProductId", c => c.Int());
            CreateIndex("dbo.OrderItems", "ProductId");
            CreateIndex("dbo.OrderItems", "SubscriptionId");
            AddForeignKey("dbo.OrderItems", "SubscriptionId", "dbo.Subscriptions", "SubscriptionId");
            AddForeignKey("dbo.OrderItems", "ProductId", "dbo.Products", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderItems", "SubscriptionId", "dbo.Subscriptions");
            DropIndex("dbo.OrderItems", new[] { "SubscriptionId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            AlterColumn("dbo.OrderItems", "ProductId", c => c.Int(nullable: false));
            DropColumn("dbo.OrderItems", "SubscriptionId");
            CreateIndex("dbo.OrderItems", "ProductId");
            AddForeignKey("dbo.OrderItems", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
    }
}
