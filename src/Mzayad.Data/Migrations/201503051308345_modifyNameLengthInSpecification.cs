namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyNameLengthInSpecification : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Specifications", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Specifications", "Name", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
