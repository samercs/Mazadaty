namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBidType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bids", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bids", "Type");
        }
    }
}
