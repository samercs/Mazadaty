namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSpecification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Specifications",
                c => new
                    {
                        SpecificationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SpecificationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Specifications");
        }
    }
}
