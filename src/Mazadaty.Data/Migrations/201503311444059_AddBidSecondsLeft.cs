namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBidSecondsLeft : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bids", "SecondsLeft", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bids", "SecondsLeft");
        }
    }
}
