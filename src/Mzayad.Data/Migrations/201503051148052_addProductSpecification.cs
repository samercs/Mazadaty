namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProductSpecification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductSpecifications",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        SpecificationId = c.Int(nullable: false),
                        Value = c.String(nullable: false,maxLength:50),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductId, t.SpecificationId })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Specifications", t => t.SpecificationId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.SpecificationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSpecifications", "SpecificationId", "dbo.Specifications");
            DropForeignKey("dbo.ProductSpecifications", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductSpecifications", new[] { "SpecificationId" });
            DropIndex("dbo.ProductSpecifications", new[] { "ProductId" });
            DropTable("dbo.ProductSpecifications");
        }
    }
}
