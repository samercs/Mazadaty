namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddXPToActivityEvents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityEvents", "XP", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityEvents", "XP");
        }
    }
}
