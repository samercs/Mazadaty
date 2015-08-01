namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrophy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trophies",
                c => new
                    {
                        TrophyId = c.Int(nullable: false, identity: true),
                        Key = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IconUrl = c.String(nullable: false),
                        XpAward = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TrophyId)
                .Index(t => t.Key, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Trophies", new[] { "Key" });
            DropTable("dbo.Trophies");
        }
    }
}
