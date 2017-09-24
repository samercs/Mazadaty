namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditPages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pages", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Pages", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Pages", "Author", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pages", "Author");
            DropColumn("dbo.Pages", "UserId");
            DropColumn("dbo.Pages", "Status");
        }
    }
}
