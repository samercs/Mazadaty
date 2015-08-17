namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAvatarUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AvatarUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AvatarUrl");
        }
    }
}
