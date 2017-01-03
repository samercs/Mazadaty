namespace Mzayad.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class FriendRequestsAndUsersFriendsTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UsersTrophies", "Users_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UsersTrophies", new[] { "Users_Id" });
            DropColumn("dbo.UsersTrophies", "UserId");
            RenameColumn(table: "dbo.UsersTrophies", name: "Users_Id", newName: "UserId");
            CreateTable(
                "dbo.FriendRequests",
                c => new
                    {
                        FriendRequestId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(),
                        DeletedUtc = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.FriendRequestId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UsersFriends",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FriendId = c.String(nullable: false, maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(),
                        DeletedUtc = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.UserId, t.FriendId })
                .ForeignKey("dbo.AspNetUsers", t => t.FriendId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.FriendId);
            
            AddColumn("dbo.AspNetUsers", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.UsersTrophies", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "ApplicationUser_Id");
            CreateIndex("dbo.UsersTrophies", "UserId");
            AddForeignKey("dbo.AspNetUsers", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.UsersTrophies", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersTrophies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersFriends", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersFriends", "FriendId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.FriendRequests", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UsersTrophies", new[] { "UserId" });
            DropIndex("dbo.UsersFriends", new[] { "FriendId" });
            DropIndex("dbo.UsersFriends", new[] { "UserId" });
            DropIndex("dbo.FriendRequests", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.UsersTrophies", "UserId", c => c.String(maxLength: 128));
            DropColumn("dbo.AspNetUsers", "ApplicationUser_Id");
            DropTable("dbo.UsersFriends");
            DropTable("dbo.FriendRequests");
            RenameColumn(table: "dbo.UsersTrophies", name: "UserId", newName: "Users_Id");
            AddColumn("dbo.UsersTrophies", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.UsersTrophies", "Users_Id");
            AddForeignKey("dbo.UsersTrophies", "Users_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
