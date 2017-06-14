namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyValueLengthInProductSpecifications : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductSpecifications", "Value", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSpecifications", "Value", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
