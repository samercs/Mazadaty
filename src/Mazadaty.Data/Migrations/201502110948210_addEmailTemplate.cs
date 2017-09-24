namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEmailTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailTemplates",
                c => new
                    {
                        EmailTemplateId = c.Int(nullable: false, identity: true),
                        TemplateType = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        Message = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EmailTemplateId)
                .Index(t => t.TemplateType, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.EmailTemplates", new[] { "TemplateType" });
            DropTable("dbo.EmailTemplates");
        }
    }
}
