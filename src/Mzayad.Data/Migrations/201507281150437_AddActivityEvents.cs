namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivityEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityEvents",
                c => new
                    {
                        ActivityEventId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        UserId = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityEventId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ActivityEvents");
        }
    }
}
