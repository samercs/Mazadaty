namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAvatar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAvatars",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        AvatarId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(),
                        DeletedUtc = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.UserId, t.AvatarId })
                .ForeignKey("dbo.Avatars", t => t.AvatarId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AvatarId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserAvatars", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserAvatars", "AvatarId", "dbo.Avatars");
            DropIndex("dbo.UserAvatars", new[] { "AvatarId" });
            DropIndex("dbo.UserAvatars", new[] { "UserId" });
            DropTable("dbo.UserAvatars");
        }
    }
}
