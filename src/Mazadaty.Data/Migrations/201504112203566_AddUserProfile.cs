namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Status = c.Int(nullable: false),
                        Gamertag = c.String(nullable: false, maxLength: 20),
                        ProfileUrl = c.String(nullable: false, maxLength: 255),
                        AvatarId = c.Int(nullable: true),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserProfileId)
                .ForeignKey("dbo.Avatars", t => t.AvatarId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.Gamertag, unique: true, name: "IX_Gamertag")
                .Index(t => t.ProfileUrl, unique: true, name: "IX_ProfileUrl")
                .Index(t => t.AvatarId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserProfiles", "AvatarId", "dbo.Avatars");
            DropIndex("dbo.UserProfiles", new[] { "AvatarId" });
            DropIndex("dbo.UserProfiles", new[] { "ProfileUrl" });
            DropIndex("dbo.UserProfiles", new[] { "Gamertag" });
            DropIndex("dbo.UserProfiles", new[] { "UserId" });
            DropTable("dbo.UserProfiles");
        }
    }
}
