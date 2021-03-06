namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAutoBidNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AutoBidNotification", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AutoBidNotification");
        }
    }
}
