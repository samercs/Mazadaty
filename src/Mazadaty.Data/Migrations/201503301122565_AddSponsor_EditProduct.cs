namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSponsor_EditProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sponsors",
                c => new
                    {
                        SponsorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SponsorId);
            
            AddColumn("dbo.Products", "SponsorId", c => c.Int());
            AddColumn("dbo.Products", "Notes", c => c.String());
            CreateIndex("dbo.Products", "SponsorId");
            AddForeignKey("dbo.Products", "SponsorId", "dbo.Sponsors", "SponsorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "SponsorId", "dbo.Sponsors");
            DropIndex("dbo.Products", new[] { "SponsorId" });
            DropColumn("dbo.Products", "Notes");
            DropColumn("dbo.Products", "SponsorId");
            DropTable("dbo.Sponsors");
        }
    }
}
