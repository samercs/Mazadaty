namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserPrizeLog : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserPrizeLogs");
            AddColumn("dbo.UserPrizeLogs", "UserPrizeLogId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.UserPrizeLogs", "CompleteDateTime", c => c.DateTime());
            AddPrimaryKey("dbo.UserPrizeLogs", "UserPrizeLogId");
            DropColumn("dbo.UserPrizeLogs", "Hash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserPrizeLogs", "Hash", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.UserPrizeLogs");
            DropColumn("dbo.UserPrizeLogs", "CompleteDateTime");
            DropColumn("dbo.UserPrizeLogs", "UserPrizeLogId");
            AddPrimaryKey("dbo.UserPrizeLogs", new[] { "PrizeId", "UserId", "Hash" });
        }
    }
}
