namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPrizeLog_IsComplete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserPrizeLogs", "IsComplete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserPrizeLogs", "IsComplete");
        }
    }
}
