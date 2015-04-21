namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserHostAddress_Bid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bids", "UserHostAddress", c => c.String(nullable: false, maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bids", "UserHostAddress");
        }
    }
}
