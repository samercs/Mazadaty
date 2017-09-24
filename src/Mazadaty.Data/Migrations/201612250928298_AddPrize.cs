namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPrize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prizes",
                c => new
                    {
                        PrizeId = c.Int(nullable: false, identity: true),
                        PrizeType = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Limit = c.Int(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 3),
                        ProductId = c.Int(),
                        SubscriptionDays = c.Int(),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(),
                        DeletedUtc = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.PrizeId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prizes", "ProductId", "dbo.Products");
            DropIndex("dbo.Prizes", new[] { "ProductId" });
            DropTable("dbo.Prizes");
        }
    }
}
