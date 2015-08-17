namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserProfile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", "AvatarId", "dbo.Avatars");
            DropForeignKey("dbo.UserProfiles", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserProfiles", new[] { "UserId" });
            DropIndex("dbo.UserProfiles", new[] { "ProfileUrl" });
            DropIndex("dbo.UserProfiles", new[] { "AvatarId" });
            DropTable("dbo.UserProfiles");

            Sql("update dbo.AspNetUsers set ProfileStatus = 2, AvatarUrl = 'https://az723232.vo.msecnd.net/avatars/abstract-q-c-512-512-2-512-635646145056125580.jpg'");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Status = c.Int(nullable: false),
                        ProfileUrl = c.String(nullable: false, maxLength: 255),
                        AvatarId = c.Int(),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserProfileId);
            
            CreateIndex("dbo.UserProfiles", "AvatarId");
            CreateIndex("dbo.UserProfiles", "ProfileUrl", unique: true);
            CreateIndex("dbo.UserProfiles", "UserId");
            AddForeignKey("dbo.UserProfiles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserProfiles", "AvatarId", "dbo.Avatars", "AvatarId");
        }
    }
}
