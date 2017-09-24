namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAvatarPremiumOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Avatars", "IsPremium", c => c.Boolean(nullable: false));
            AddColumn("dbo.Avatars", "Token", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Avatars", "Token");
            DropColumn("dbo.Avatars", "IsPremium");
        }
    }
}
