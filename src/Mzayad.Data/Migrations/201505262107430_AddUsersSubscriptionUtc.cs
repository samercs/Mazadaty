namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsersSubscriptionUtc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SubscriptionUtc", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SubscriptionUtc");
        }
    }
}
