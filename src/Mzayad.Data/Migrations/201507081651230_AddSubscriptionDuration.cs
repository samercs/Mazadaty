namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubscriptionDuration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "Duration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscriptions", "Duration");
        }
    }
}
