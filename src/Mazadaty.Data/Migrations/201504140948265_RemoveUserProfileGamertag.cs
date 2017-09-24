namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserProfileGamertag : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserProfiles", new[] { "Gamertag" });
            DropColumn("dbo.UserProfiles", "Gamertag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "Gamertag", c => c.String(nullable: false, maxLength: 20));
            CreateIndex("dbo.UserProfiles", "Gamertag", unique: true);
        }
    }
}
