namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFriendsAndMessages : DbMigration
    {
        public override void Up()
        {
            Sql("delete from dbo.UsersTrophies");

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
                        FriendId = c.String(nullable: false, maxLength: 128),
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
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Body = c.String(nullable: false),
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        IsNew = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(),
                        DeletedUtc = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.MessageId)
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
                .ForeignKey("dbo.AspNetUsers", t => t.FriendId, cascadeDelete: true)
                .Index(t => t.FriendId);
            
            AlterColumn("dbo.UsersTrophies", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.UsersTrophies", "UserId");
            AddForeignKey("dbo.UsersTrophies", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersTrophies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersFriends", "FriendId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.FriendRequests", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UsersTrophies", new[] { "UserId" });
            DropIndex("dbo.UsersFriends", new[] { "FriendId" });
            DropIndex("dbo.Messages", new[] { "UserId" });
            DropIndex("dbo.FriendRequests", new[] { "UserId" });
            AlterColumn("dbo.UsersTrophies", "UserId", c => c.String(maxLength: 128));
            DropTable("dbo.UsersFriends");
            DropTable("dbo.Messages");
            DropTable("dbo.FriendRequests");
            RenameColumn(table: "dbo.UsersTrophies", name: "UserId", newName: "Users_Id");
            AddColumn("dbo.UsersTrophies", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.UsersTrophies", "Users_Id");
            AddForeignKey("dbo.UsersTrophies", "Users_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
