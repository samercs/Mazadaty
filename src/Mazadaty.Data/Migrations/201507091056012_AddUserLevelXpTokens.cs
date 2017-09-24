namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserLevelXpTokens : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Level", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Xp", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Tokens", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Tokens");
            DropColumn("dbo.AspNetUsers", "Xp");
            DropColumn("dbo.AspNetUsers", "Level");
        }
    }
}
