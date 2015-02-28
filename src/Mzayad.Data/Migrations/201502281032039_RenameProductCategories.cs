namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameProductCategories : DbMigration
    {
        public override void Up()
        {
            RenameTable("ProductCategories", "CategoryProducts");
        }
        
        public override void Down()
        {
        }
    }
}
