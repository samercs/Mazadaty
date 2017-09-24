namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShippingAddress_Order_OrderItem_OrderLog : DbMigration
    {
        public override void Up()
        {
            
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        OrderItemId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Name = c.String(),
                        ItemPrice = c.Decimal(nullable: false, precision: 18, scale: 3),
                        Quantity = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.OrderLogs",
                c => new
                    {
                        OrderLogId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Status = c.Int(nullable: false),
                        UserHostAddress = c.String(nullable: false, maxLength: 15),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderLogId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.OrderId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        AddressId = c.Int(),
                        Type = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 3),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 3),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PaymentMethod = c.Int(),
                        SubmittedUtc = c.DateTime(),
                        ShippedUtc = c.DateTime(),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.ShipmentAddress", t => t.AddressId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.ShipmentAddress",
                c => new
                    {
                        AddressId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .Index(t => t.AddressId);
            
            AddColumn("dbo.Bids", "UserHostAddress", c => c.String(nullable: false, maxLength: 15));
            
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        AddressLine1 = c.String(nullable: false),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        CityArea = c.String(nullable: false),
                        StateProvince = c.String(),
                        PostalCode = c.String(),
                        CountryCode = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId);
            
            DropForeignKey("dbo.ShipmentAddress", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.OrderLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderLogs", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "AddressId", "dbo.ShipmentAddress");
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropIndex("dbo.ShipmentAddress", new[] { "AddressId" });
            DropIndex("dbo.Orders", new[] { "AddressId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.OrderLogs", new[] { "UserId" });
            DropIndex("dbo.OrderLogs", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropColumn("dbo.Bids", "UserHostAddress");
            DropTable("dbo.ShipmentAddress");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderLogs");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Addresses");
        }
    }
}
