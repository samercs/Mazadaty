namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSessionLogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SessionLogs",
                c => new
                    {
                        SessionLogId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IP = c.String(),
                        Browser = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SessionLogId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SessionLogs", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SessionLogs", new[] { "UserId" });
            DropTable("dbo.SessionLogs");
        }
    }
}
