namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSplashAds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SplashAds",
                c => new
                    {
                        SplashAdId = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Weight = c.Int(nullable: false),
                        SortOrder = c.Double(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SplashAdId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SplashAds");
        }
    }
}
