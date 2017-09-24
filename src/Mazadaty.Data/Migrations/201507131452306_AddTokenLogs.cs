namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTokenLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TokenLogs",
                c => new
                    {
                        TokenLogId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ModifiedByUserId = c.String(nullable: false, maxLength: 128),
                        OriginalTokenAmount = c.Int(),
                        ModifiedTokenAmount = c.Int(nullable: false),
                        UserHostAddress = c.String(nullable: false, maxLength: 15),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TokenLogId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.ModifiedByUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TokenLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TokenLogs", "ModifiedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.TokenLogs", new[] { "ModifiedByUserId" });
            DropIndex("dbo.TokenLogs", new[] { "UserId" });
            DropTable("dbo.TokenLogs");
        }
    }
}
