namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFriendToFriendsRequest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FriendRequests", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.FriendRequests", new[] { "UserId" });
            AddColumn("dbo.FriendRequests", "FriendId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.FriendRequests", "FriendId");
            AddForeignKey("dbo.FriendRequests", "FriendId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FriendRequests", "FriendId", "dbo.AspNetUsers");
            DropIndex("dbo.FriendRequests", new[] { "FriendId" });
            DropColumn("dbo.FriendRequests", "FriendId");
            CreateIndex("dbo.FriendRequests", "UserId");
            AddForeignKey("dbo.FriendRequests", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
