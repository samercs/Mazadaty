namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategorySlug : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Slug", c => c.String(nullable: false, maxLength: 50));

            Sql("update dbo.Categories set Slug = cast(CategoryId as nvarchar(50));");

            CreateIndex("dbo.Categories", "Slug", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Categories", new[] { "Slug" });
            DropColumn("dbo.Categories", "Slug");
        }
    }
}
