namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetSubscriptionExpirationUtcNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Subscriptions", "ExpirationUtc", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subscriptions", "ExpirationUtc", c => c.DateTime(nullable: false));
        }
    }
}
