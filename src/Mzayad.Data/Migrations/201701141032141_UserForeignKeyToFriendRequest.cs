namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserForeignKeyToFriendRequest : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.FriendRequests", "UserId");
            RenameColumn(table: "dbo.FriendRequests", name: "FriendId", newName: "UserId");
            RenameIndex(table: "dbo.FriendRequests", name: "IX_FriendId", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.FriendRequests", name: "IX_UserId", newName: "IX_FriendId");
            RenameColumn(table: "dbo.FriendRequests", name: "UserId", newName: "FriendId");
            AddColumn("dbo.FriendRequests", "UserId", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
