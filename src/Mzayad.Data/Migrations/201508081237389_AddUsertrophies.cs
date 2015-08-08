namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsertrophies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsersTrophies",
                c => new
                    {
                        UserTrohyId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        TrophyId = c.Int(nullable: false),
                        XpAwarded = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        Users_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserTrohyId)
                .ForeignKey("dbo.Trophies", t => t.TrophyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Users_Id)
                .Index(t => t.TrophyId)
                .Index(t => t.Users_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersTrophies", "Users_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersTrophies", "TrophyId", "dbo.Trophies");
            DropIndex("dbo.UsersTrophies", new[] { "Users_Id" });
            DropIndex("dbo.UsersTrophies", new[] { "TrophyId" });
            DropTable("dbo.UsersTrophies");
        }
    }
}
