namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHijriUniqueIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.IslamicCalendars", "HijriYear", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.IslamicCalendars", new[] { "HijriYear" });
        }
    }
}
