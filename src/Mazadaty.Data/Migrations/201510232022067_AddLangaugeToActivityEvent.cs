namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLangaugeToActivityEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityEvents", "Language", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityEvents", "Language");
        }
    }
}
