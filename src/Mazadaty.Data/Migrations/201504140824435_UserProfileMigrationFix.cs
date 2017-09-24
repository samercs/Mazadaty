namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfileMigrationFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", "AvatarId", "dbo.Avatars");
            DropIndex("dbo.UserProfiles", new[] { "ProfileUrl" });
            DropIndex("dbo.UserProfiles", new[] { "AvatarId" });
            AlterColumn("dbo.UserProfiles", "ProfileUrl", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.UserProfiles", "AvatarId", c => c.Int());
            CreateIndex("dbo.UserProfiles", "ProfileUrl", unique: true);
            CreateIndex("dbo.UserProfiles", "AvatarId");
            AddForeignKey("dbo.UserProfiles", "AvatarId", "dbo.Avatars", "AvatarId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "AvatarId", "dbo.Avatars");
            DropIndex("dbo.UserProfiles", new[] { "AvatarId" });
            DropIndex("dbo.UserProfiles", new[] { "ProfileUrl" });
            AlterColumn("dbo.UserProfiles", "AvatarId", c => c.Int(nullable: false));
            AlterColumn("dbo.UserProfiles", "ProfileUrl", c => c.String(nullable: false));
            CreateIndex("dbo.UserProfiles", "AvatarId");
            CreateIndex("dbo.UserProfiles", "ProfileUrl", unique: true);
            AddForeignKey("dbo.UserProfiles", "AvatarId", "dbo.Avatars", "AvatarId", cascadeDelete: true);
        }
    }
}
