namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropActivityLogs : DbMigration
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
            
            DropColumn("dbo.OrderLogs", "UserHostAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderLogs", "UserHostAddress", c => c.String(nullable: false, maxLength: 15));
            DropTable("dbo.ActivityEvents");
        }
    }
}
